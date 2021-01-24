namespace Understand.Fsharp.Monad.XState

module State =
  type Label = string

  type VitalForce =
    { units: int }

  let getVitalForce vitalForce =
    let oneUnit = { units = 1 }
    let remaining = { units = vitalForce.units - 1 }

    oneUnit, remaining

  type DeafLeftLeg = DeafLeftLeg of Label

  type LiveLeftLeg = LiveLeftLeg of Label * VitalForce

  type MakeLiveLeftLeg = DeafLeftLeg * VitalForce -> (LiveLeftLeg * VitalForce)

  type M<'LiveBodyPart> =
    M of (VitalForce -> 'LiveBodyPart * VitalForce)

  let runM (M f) vitalForce = f vitalForce

  let makeLiveLeftLeg deadLeftLeg :M<LiveLeftLeg> =
    let becomeAlive vitalForce =
      let (DeafLeftLeg label) = deadLeftLeg
      let oneUnit, remainingVitalForce = getVitalForce vitalForce
      let liveLeftLeg = LiveLeftLeg(label, oneUnit)

      liveLeftLeg, remainingVitalForce
    M becomeAlive

  let ``test the left leg``() =
    let deadLeftLeg = DeafLeftLeg "Boris"
    let leftLegM = makeLiveLeftLeg deadLeftLeg

    let vf = { units = 10 }
    let liveLeftLeg, remainingAfterLeftLeg = runM leftLegM vf

    liveLeftLeg

  type DeadLeftBrokenArm = DeadLeftBrokenArm of Label

  // A live version of the broken arm.
   type LiveLeftBrokenArm = LiveLeftBrokenArm of Label * VitalForce

  // A live version of a heathly arm, with no dead version available
  type LiveLeftArm = LiveLeftArm of Label * VitalForce

  // An operation that can turn a broken left arm into a heathly left arm
  type HealBrokenArm = LiveLeftBrokenArm -> LiveLeftArm






