; olaXuuuuuh
(set-info :status unknown)
(set-logic QF_AUFLIRA)
(declare-fun |HandLeft Norm| () Real)
(declare-fun |HandLeft Z| () Real)
(declare-fun |SpineMid Norm| () Real)
(declare-fun |SpineMid Z| () Real)
(declare-fun |SpineShoulder Norm| () Real)
(declare-fun |SpineShoulder Z| () Real)
(declare-fun |ShoulderLeft Norm| () Real)
(declare-fun |ShoulderLeft Z| () Real)
(declare-fun |ElbowLeft Norm| () Real)
(declare-fun |ElbowLeft Z| () Real)
(declare-fun |WristLeft Norm| () Real)
(declare-fun |Head Norm| () Real)
(declare-fun |Neck Norm| () Real)
(declare-fun |Neck Z| () Real)
(declare-fun |HandLeft Y| () Real)
(declare-fun |SpineMid Y| () Real)
(declare-fun |SpineShoulder Y| () Real)
(declare-fun |ShoulderLeft Y| () Real)
(declare-fun |ElbowLeft Y| () Real)
(declare-fun |Neck Y| () Real)
(declare-fun |HandLeft X| () Real)
(declare-fun |SpineMid X| () Real)
(declare-fun |SpineShoulder X| () Real)
(declare-fun |ShoulderLeft X| () Real)
(declare-fun |ElbowLeft X| () Real)
(declare-fun |Neck X| () Real)
(declare-fun |HandRight Norm| () Real)
(declare-fun |HandRight Z| () Real)
(declare-fun |ShoulderRight Norm| () Real)
(declare-fun |ShoulderRight Z| () Real)
(declare-fun |ElbowRight Norm| () Real)
(declare-fun |ElbowRight Z| () Real)
(declare-fun |WristRight Norm| () Real)
(declare-fun |HandRight Y| () Real)
(declare-fun |ShoulderRight Y| () Real)
(declare-fun |ElbowRight Y| () Real)
(declare-fun |HandRight X| () Real)
(declare-fun |ShoulderRight X| () Real)
(declare-fun |ElbowRight X| () Real)
(declare-fun |ThumbRight Z| () Real)
(declare-fun |ThumbRight Y| () Real)
(declare-fun |ThumbRight X| () Real)
(assert
(let ((?x5357 (* |HandLeft Z| |HandLeft Norm|)))
(let ((?x159 (* |SpineMid Z| |SpineMid Norm|)))
(let ((?x435 (* |SpineShoulder Z| |SpineShoulder Norm|)))
(let ((?x420 (* |ShoulderLeft Z| |ShoulderLeft Norm|)))
(let ((?x516 (* |ElbowLeft Z| |ElbowLeft Norm|)))
(let ((?x169 (* (/ 433.0 500.0) 0.0)))
(let ((?x170 (* (~ (/ 1.0 2.0)) (~ 1.0))))
(let ((?x3219 (+ ?x170 ?x169)))
(let ((?x172 (* (/ 1.0 2.0) (~ 1.0))))
(let ((?x5117 (+ ?x172 ?x169)))
(let (($x134 (>= (~ 1.0) 0.0)))
(let ((?x4265 (ite $x134 ?x5117 ?x3219)))
(let ((?x179 (* (~ (/ 1.0 2.0)) 0.0)))
(let ((?x3898 (+ ?x179 ?x169)))
(let ((?x176 (* (/ 1.0 2.0) 0.0)))
(let ((?x5101 (+ ?x176 ?x169)))
(let (($x112 (>= 0.0 0.0)))
(let ((?x5103 (ite $x112 ?x5101 ?x3898)))
(let ((?x135 (- 0.0 (~ 1.0))))
(let ((?x136 (ite $x134 (~ 1.0) ?x135)))
(let ((?x113 (- 0.0 0.0)))
(let ((?x114 (ite $x112 0.0 ?x113)))
(let (($x4784 (>= ?x114 ?x136)))
(let ((?x5407 (ite $x4784 ?x5103 ?x4265)))
(let ((?x4357 (+ 0.0 ?x5357)))
(let ((?x6191 (+ (+ (+ (+ (+ ?x4357 (* ?x5407 |WristLeft Norm|)) ?x516) ?x420) ?x435) ?x159)))
(let ((?x187 (* (/ 939.0 1000.0) 0.0)))
(let ((?x4166 (* (~ (/ 171.0 500.0)) 1.0)))
(let ((?x4618 (+ ?x4166 ?x187)))
(let ((?x5110 (* (/ 171.0 500.0) 1.0)))
(let ((?x3672 (+ ?x5110 ?x187)))
(let (($x350 (>= 1.0 0.0)))
(let ((?x4560 (ite $x350 ?x3672 ?x4618)))
(let ((?x197 (* (~ (/ 171.0 500.0)) 0.0)))
(let ((?x313 (+ ?x197 ?x187)))
(let ((?x194 (* (/ 171.0 500.0) 0.0)))
(let ((?x314 (+ ?x194 ?x187)))
(let ((?x385 (ite $x112 ?x314 ?x313)))
(let ((?x351 (- 0.0 1.0)))
(let ((?x352 (ite $x350 1.0 ?x351)))
(let (($x5024 (>= ?x114 ?x352)))
(let ((?x5485 (ite $x5024 ?x385 ?x4560)))
(let ((?x6495 (* ?x5485 |Head Norm|)))
(let ((?x724 (* |Neck Z| |Neck Norm|)))
(let ((?x7221 (+ 0.0 ?x6495)))
(let ((?x7172 (+ ?x7221 ?x724)))
(let ((?x6410 (+ ?x7172 ?x435)))
(let ((?x6963 (+ ?x6410 ?x159)))
(let ((?x6629 (+ ?x6963 ?x6495)))
(let ((?x5014 (- ?x6629 (+ ?x6191 ?x5357))))
(let ((?x6347 (ite (>= ?x5014 0.0) ?x5014 (- 0.0 ?x5014))))
(let ((?x5556 (* |HandLeft Y| |HandLeft Norm|)))
(let ((?x443 (* |SpineMid Y| |SpineMid Norm|)))
(let ((?x427 (* |SpineShoulder Y| |SpineShoulder Norm|)))
(let ((?x417 (* |ShoulderLeft Y| |ShoulderLeft Norm|)))
(let ((?x5520 (* |ElbowLeft Y| |ElbowLeft Norm|)))
(let ((?x177 (* (/ 433.0 500.0) (~ 1.0))))
(let ((?x4795 (+ ?x177 ?x176)))
(let ((?x5420 (+ ?x177 ?x179)))
(let ((?x4680 (ite $x134 ?x5420 ?x4795)))
(let ((?x4160 (ite $x4784 (~ 1.0) ?x4680)))
(let ((?x3426 (+ 0.0 ?x5556)))
(let ((?x7093 (+ (+ (+ (+ (+ ?x3426 (* ?x4160 |WristLeft Norm|)) ?x5520) ?x417) ?x427) ?x443)))
(let ((?x3828 (* (/ 939.0 1000.0) 1.0)))
(let ((?x5439 (+ ?x3828 ?x194)))
(let ((?x4064 (+ ?x3828 ?x197)))
(let ((?x4293 (ite $x350 ?x4064 ?x5439)))
(let ((?x4699 (ite $x5024 1.0 ?x4293)))
(let ((?x7073 (* ?x4699 |Head Norm|)))
(let ((?x736 (* |Neck Y| |Neck Norm|)))
(let ((?x6069 (+ 0.0 ?x7073)))
(let ((?x6229 (+ ?x6069 ?x736)))
(let ((?x7183 (+ ?x6229 ?x427)))
(let ((?x6970 (+ ?x7183 ?x443)))
(let ((?x6237 (+ ?x6970 ?x7073)))
(let ((?x7199 (- ?x6237 (+ ?x7093 ?x5556))))
(let ((?x6327 (ite (>= ?x7199 0.0) ?x7199 (- 0.0 ?x7199))))
(let ((?x5403 (* |HandLeft X| |HandLeft Norm|)))
(let ((?x441 (* |SpineMid X| |SpineMid Norm|)))
(let ((?x426 (* |SpineShoulder X| |SpineShoulder Norm|)))
(let ((?x416 (* |ShoulderLeft X| |ShoulderLeft Norm|)))
(let ((?x4877 (* |ElbowLeft X| |ElbowLeft Norm|)))
(let ((?x5213 (+ ?x169 ?x176)))
(let ((?x4162 (+ ?x169 ?x179)))
(let ((?x5471 (ite $x112 ?x4162 ?x5213)))
(let ((?x5198 (ite $x4784 ?x5471 0.0)))
(let ((?x4526 (+ 0.0 ?x5403)))
(let ((?x6382 (+ (+ (+ (+ (+ ?x4526 (* ?x5198 |WristLeft Norm|)) ?x4877) ?x416) ?x426) ?x441)))
(let ((?x321 (+ ?x187 ?x194)))
(let ((?x322 (+ ?x187 ?x197)))
(let ((?x389 (ite $x112 ?x322 ?x321)))
(let ((?x5576 (ite $x5024 ?x389 0.0)))
(let ((?x5994 (* ?x5576 |Head Norm|)))
(let ((?x526 (* |Neck X| |Neck Norm|)))
(let ((?x6202 (+ 0.0 ?x5994)))
(let ((?x7168 (+ ?x6202 ?x526)))
(let ((?x6328 (+ ?x7168 ?x426)))
(let ((?x6958 (+ ?x6328 ?x441)))
(let ((?x6220 (+ ?x6958 ?x5994)))
(let ((?x6113 (- ?x6220 (+ ?x6382 ?x5403))))
(let ((?x6019 (ite (>= ?x6113 0.0) ?x6113 (- 0.0 ?x6113))))
(let ((?x6402 (ite (>= ?x6019 ?x6327) ?x6019 ?x6327)))
(let ((?x3920 (* |HandRight Z| |HandRight Norm|)))
(let ((?x4101 (* |ShoulderRight Z| |ShoulderRight Norm|)))
(let ((?x5251 (* |ElbowRight Z| |ElbowRight Norm|)))
(let ((?x5946 (+ (+ (+ (+ (+ 0.0 ?x3920) (* ?x5407 |WristRight Norm|)) ?x5251) ?x4101) ?x435)))
(let ((?x6931 (- ?x6629 (+ (+ ?x5946 ?x159) ?x3920))))
(let ((?x6445 (ite (>= ?x6931 0.0) ?x6931 (- 0.0 ?x6931))))
(let ((?x4421 (* |HandRight Y| |HandRight Norm|)))
(let ((?x5302 (* |ShoulderRight Y| |ShoulderRight Norm|)))
(let ((?x5405 (* |ElbowRight Y| |ElbowRight Norm|)))
(let ((?x6814 (+ (+ (+ (+ (+ 0.0 ?x4421) (* ?x4160 |WristRight Norm|)) ?x5405) ?x5302) ?x427)))
(let ((?x6917 (- ?x6237 (+ (+ ?x6814 ?x443) ?x4421))))
(let ((?x6545 (ite (>= ?x6917 0.0) ?x6917 (- 0.0 ?x6917))))
(let ((?x5582 (* |HandRight X| |HandRight Norm|)))
(let ((?x5113 (* |ShoulderRight X| |ShoulderRight Norm|)))
(let ((?x5038 (* |ElbowRight X| |ElbowRight Norm|)))
(let ((?x6732 (+ (+ (+ (+ (+ 0.0 ?x5582) (* ?x5198 |WristRight Norm|)) ?x5038) ?x5113) ?x426)))
(let ((?x6914 (- ?x6220 (+ (+ ?x6732 ?x441) ?x5582))))
(let ((?x4973 (ite (>= ?x6914 0.0) ?x6914 (- 0.0 ?x6914))))
(let ((?x6563 (ite (>= ?x4973 ?x6545) ?x4973 ?x6545)))
(let (($x6527 (and (and true (< (ite (>= ?x6563 ?x6445) ?x6563 ?x6445) (/ 1.0 5.0))) (< (ite (>= ?x6402 ?x6347) ?x6402 ?x6347) (/ 1.0 5.0)))))
(let (($x1877 (and true true)))
(let ((?x3957 (- |ThumbRight Z| |ThumbRight Z|)))
(let ((?x5304 (- 0.0 ?x3957)))
(let (($x5305 (>= ?x3957 0.0)))
(let ((?x5307 (ite $x5305 ?x3957 ?x5304)))
(let ((?x2491 (- |ThumbRight Y| |ThumbRight Y|)))
(let ((?x5299 (- 0.0 ?x2491)))
(let (($x5279 (>= ?x2491 0.0)))
(let ((?x5303 (ite $x5279 ?x2491 ?x5299)))
(let ((?x5477 (- |ThumbRight X| |ThumbRight X|)))
(let ((?x5278 (- 0.0 ?x5477)))
(let (($x3697 (>= ?x5477 0.0)))
(let ((?x5294 (ite $x3697 ?x5477 ?x5278)))
(let (($x5085 (>= ?x5294 ?x5303)))
(let ((?x4985 (ite $x5085 ?x5294 ?x5303)))
(let (($x4577 (>= ?x4985 ?x5307)))
(let ((?x3992 (ite $x4577 ?x4985 ?x5307)))
(let (($x5034 (< ?x3992 15.0)))
(let (($x1404 (and $x5034)))
(and $x1404 (and (and (and (and $x1877 true) true) true) true) (and $x1877 $x6527))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))))
(check-sat)