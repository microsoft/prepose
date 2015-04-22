using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Z3Experiments
{
    [TestMethod]
        public void TestBitVectorOps()
        {
            Context z3 = new Context();
            var bv16 = z3.MkBitVecSort(16);
            var c = (BitVecExpr)z3.MkConst("c",bv16);
            var _3 = (BitVecExpr)z3.MkNumeral(3, bv16);
            var _7 = (BitVecExpr)z3.MkNumeral(7, bv16);
            var _1 = (BitVecExpr)z3.MkNumeral(1, bv16);
            var c_and_7 = z3.MkBVAND(c, _7);
            //((1 + (c & 7)) & 3)
            var t = z3.MkBVAND(z3.MkBVAdd(_1, c_and_7), _3);
            var s = t.Simplify(); //comes out as: (1 + (c & 3))
            var t_neq_s = z3.MkNot(z3.MkEq(t, s));
            var solv =z3.MkSolver();
            solv.Assert(t_neq_s);
            Assert.AreEqual(Status.UNSATISFIABLE, solv.Check());
        }

        [TestMethod]
        public void TupleTest()
        {
            Z3Provider z3p = new Z3Provider();
            //create the tuple sort for mouth
            FuncDecl mouth;
            FuncDecl[] mouth_accessors;
            var MOUTH = z3p.MkTupleSort("MOUTH", new string[] { "open", "teeth" }, new Sort[] { z3p.BoolSort, z3p.IntSort }, out mouth, out mouth_accessors);
            Func<Expr,Expr,Expr> mk_mouth = ((o,t) => z3p.MkApp(mouth, o, t));
            Func<Expr,Expr> get_open = (m => z3p.MkApp(mouth_accessors[0], m));
            Func<Expr,Expr> get_teeth = (m => z3p.MkApp(mouth_accessors[1], m));
            //create the tuple sort for nose
            FuncDecl nose;
            FuncDecl[] nose_accessors;
            var NOSE = z3p.MkTupleSort("NOSE", new string[] { "size" }, new Sort[] { z3p.IntSort }, out nose, out nose_accessors);
            Func<Expr,Expr> mk_nose = (s => z3p.MkApp(nose, s));
            Func<Expr,Expr> get_size = (n => z3p.MkApp(nose_accessors[0], n));
            //create the tuple sort for head
            FuncDecl head;
            FuncDecl[] head_accessors;
            var HEAD = z3p.MkTupleSort("HEAD", new string[] { "bald", "nose", "mouth" }, new Sort[] { z3p.BoolSort, NOSE, MOUTH }, out head, out head_accessors);
            Func<Expr,Expr,Expr,Expr> mk_head = ((b,n,m) => z3p.MkApp(head, b,n,m));
            Func<Expr,Expr> get_bald = (h => z3p.MkApp(head_accessors[0], h));
            Func<Expr,Expr> get_nose = (h => z3p.MkApp(head_accessors[1], h));
            Func<Expr,Expr> get_mouth = (h => z3p.MkApp(head_accessors[2], h));
            //------------------------ 
            // create a transformation "punch" from HEAD tp HEAD that removes k teeth, k is  the second parameter of the transformation
            var punch = z3p.MkFuncDecl("punch", new Sort[]{HEAD, z3p.IntSort}, HEAD);
            var x = z3p.MkBound(0, HEAD);        // <-- bound variable with deBrujn index 0, this is the input HEAD
            var y = z3p.MkBound(1, z3p.IntSort); // <-- bound variable with deBrujn index 1, this is the n parameter
            //this is the actual transformation of x that removes one tooth
            var new_mouth = mk_mouth(get_open(get_mouth(x)), z3p.MkSub(get_teeth(get_mouth(x)), y));
            var old_nose = get_nose(x);
            var old_bald = get_bald(x);
            var punch_def = mk_head(old_bald, old_nose,new_mouth);
            var punch_axiom = z3p.MkEqForall(z3p.MkApp(punch, x , y), punch_def, x, y);
            Func<Expr,Expr,Expr> punch_app = ((h,k) => z3p.MkApp(punch, h,k));
            z3p.AssertCnstr(punch_axiom);
            //------------------------ 
            // create a transformation "hit" from HEAD tp HEAD that doubles the size of the nose
            var hit = z3p.MkFuncDecl("hit", HEAD, HEAD);  
            var hit_def = mk_head(get_bald(x), mk_nose(z3p.MkMul(z3p.MkInt(2),get_size(get_nose(x)))), get_mouth(x));
            var hit_axiom = z3p.MkEqForall(z3p.MkApp(hit, x), hit_def, x);
            Func<Expr,Expr> hit_app = (h => z3p.MkApp(hit, h));
            z3p.AssertCnstr(hit_axiom);
            //-------------------------------
            // Analysis
            var H = z3p.MkConst("H", HEAD);
            var N = z3p.MkConst("N", z3p.IntSort);
            // check that hit and punch commute
            z3p.Push();
            z3p.AssertCnstr(z3p.MkNeq(punch_app(hit_app(H), N), hit_app(punch_app(H, N))));
            Status status = z3p.Check(); //here status must be UNSATISFIABLE
            z3p.Pop(); //remove the temporary context
            //check that hit is not idempotent
            z3p.Push();
            z3p.AssertCnstr(z3p.MkNeq(hit_app(hit_app(H)), hit_app(H)));
            status = z3p.Check(); //here status must not be UNSATISFIABLE (it is UNKNOWN due to use of axioms)
            var model1 = z3p.Z3S.Model;
            var witness1 = model1.Evaluate(H, true);   //a concrete instance of HEAD that shows when hitting twice is not the same as hitting once
            z3p.Pop();
            //but it is possible that hitting twice does no harm (when nose has size 0)
            z3p.Push();
            z3p.AssertCnstr(z3p.MkEq(hit_app(hit_app(H)), hit_app(H)));
            status = z3p.Check(); 
            var model2 = z3p.Z3S.Model;
            var witness2 = model2.Evaluate(H, true);  
            z3p.Pop();
        }
    }


    public class MyExpr
    {
        public Expr expr;
        Context z3;

        public MyExpr(Expr expr, Context z3)
        {
            this.z3 = z3;
            this.expr = expr;
        }

        public static MyExpr operator +(MyExpr expr1, MyExpr expr2)
        {
            if (expr1.z3 != expr2.z3)
                throw new Exception("Context mismatch");
            if (expr1.expr.Sort.Equals(expr2.expr.Sort))
                throw new Exception("Sort mismatch");
            var sort = expr1.expr.Sort;
            var z3 = expr1.z3;
            if (sort.SortKind == Z3_sort_kind.Z3_INT_SORT || sort.SortKind == Z3_sort_kind.Z3_REAL_SORT)
            {
                if (!(expr1.expr is ArithExpr))
                    throw new Exception("Not arithmetic expression");
                if (!(expr2.expr is ArithExpr))
                    throw new Exception("Not arithmetic expression");
                return new MyExpr(z3.MkAdd((ArithExpr)expr1.expr, (ArithExpr)expr2.expr), z3);
            }
            if (sort.SortKind == Z3_sort_kind.Z3_BV_SORT)
            {
                if (!(expr1.expr is BitVecExpr))
                    throw new Exception("Not bitvector expression");
                if (!(expr1.expr is BitVecExpr))
                    throw new Exception("Not bitvector expression");
                return new MyExpr(z3.MkBVAdd((BitVecExpr)expr1.expr, (BitVecExpr)expr2.expr), z3);
            }
            throw new NotImplementedException("operator '+' is not implemented for " + sort.ToString());
        }

        public static MyExpr operator <(MyExpr expr1, MyExpr expr2)
        {
            if (expr1.z3 != expr2.z3)
                throw new Exception("Context mismatch");
            if (expr1.expr.Sort.Equals(expr2.expr.Sort))
                throw new Exception("Sort mismatch");
            var sort = expr1.expr.Sort;
            var z3 = expr1.z3;
            if (sort.SortKind == Z3_sort_kind.Z3_INT_SORT || sort.SortKind == Z3_sort_kind.Z3_REAL_SORT)
            {
                if (!(expr1.expr is ArithExpr))
                    throw new Exception("Not arithmetic expression");
                if (!(expr2.expr is ArithExpr))
                    throw new Exception("Not arithmetic expression");
                return new MyExpr(z3.MkLt((ArithExpr)expr1.expr, (ArithExpr)expr2.expr), z3);
            }
            if (sort.SortKind == Z3_sort_kind.Z3_BV_SORT)
            {
                if (!(expr1.expr is BitVecExpr))
                    throw new Exception("Not bitvector expression");
                if (!(expr1.expr is BitVecExpr))
                    throw new Exception("Not bitvector expression");
                return new MyExpr(z3.MkBVSLT((BitVecExpr)expr1.expr, (BitVecExpr)expr2.expr), z3);
            }
            throw new NotImplementedException("operator '<' is not implemented for " + sort.ToString());
        }
    }

    public class Head
    {
        Z3Provider z3p;
        TupleSort HEAD;
        TupleSort MOUTH;
        TupleSort NOSE;
        Expr head;

        Expr bald;
        Mouth mouth;
        Nose nose;

        public Head(Z3Provider z3p, Expr head, TupleSort HEAD, TupleSort MOUTH, TupleSort NOSE)
        {
            if (!(head.Sort.Equals(HEAD)))
                throw new Exception("Wrong sort");

            this.z3p = z3p;

            this.head = head;
            this.HEAD = HEAD;
            this.MOUTH = MOUTH;
            this.NOSE = NOSE;
        }

        public Expr Expr {
            get {return head;}
        }

        public Expr Bald
        {
            get
            {
                return z3p.MkApp(HEAD.FieldDecls[0],head);
            }
        }

        public Mouth Mouth
        {
        }

    }

    public class Mouth
    {
        public bool open;
        public int teeth;
    }

    public class Nose
    {
    }
}
