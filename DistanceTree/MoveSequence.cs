using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.GestureRecognizer
{
    using System.IO;
    using System.Windows;
    using Microsoft.Automata;
    using Microsoft.Automata.Internal;

    public class MoveSequence
    {

        public Automaton<BvSet> moveAutomaton;

        /*public int size;
        public List<Move>[,] moves;
        public List<int> finalStates;*/
        public CharSetSolver solver;

        public int currentState;
        public List<int> deadStates;


        public MoveSequence(string regex)
        {
            solver = new CharSetSolver(BitWidth.BV7);
            moveAutomaton = solver.Convert("^(" + regex + ")$").Determinize(solver).Minimize(solver);
            currentState = 0;
            //solver.ShowGraph(moveAutomaton, "D");
            //ComputeDeadStates();

        }

        public void MakeOneStep(Transduction move)
        {
          char c = TransductionUtil.ToChar(move);
            IEnumerable<Move<BvSet>> movesFromCurrent = moveAutomaton.GetMovesFrom(currentState);

            foreach (Move<BvSet> m in movesFromCurrent)
            {
                if (!solver.MkAnd(m.Condition, solver.MkCharConstraint(true, c)).IsEmpty)
                {
                    currentState = m.TargetState;
                    return;
                }
            }

            throw new Exception("Move not available");
        }

        public List<Transduction> AvailableMoves()
        {
            IEnumerable<Move<BvSet>> movesFromCurrent = moveAutomaton.GetMovesFrom(currentState);
            BvSet availableMoves = solver.MkAnd(solver.MkCharConstraint(true, '2'), solver.MkCharConstraint(true, '3'));
            List<char> charl;
            foreach (Move<BvSet> m in movesFromCurrent)
            {
                charl = solver.GenerateAllCharacters(m.Condition, false).ToList();
                availableMoves = solver.MkOr(m.Condition, availableMoves);
            }
            List<Transduction> movel = new List<Transduction>();
            IEnumerable<char> chare = solver.GenerateAllCharacters(availableMoves, false);
            //charl = chare.ToList();
            foreach (Char c in chare)
            {
              movel.Add(TransductionUtil.ToMove(c));
            }

            return movel;
        }


        public bool isFinal()
        {
            return moveAutomaton.IsFinalState(currentState);
        }
    }
}
