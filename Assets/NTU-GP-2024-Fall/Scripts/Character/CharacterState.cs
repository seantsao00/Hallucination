using UnityEngine;

public class CharacterState {
    public interface ICharacterState {
        void HandleStateChange(Character character, bool isActive);
    }

    public abstract class CharacterStateBase : ICharacterState {
        public virtual void HandleStateChange(Character character, bool isActive) { }
    }

    public class Free : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.CurrentMovement.SetNormal();
            } else {
            }
        }
    }

    public class Climbing : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.gameObject.layer = LayerMask.NameToLayer("AheadGround");
                character.Rb.gravityScale = 0;
                character.Rb.velocity = new Vector2(0, character.Rb.velocity.y);
            } else {
                character.gameObject.layer = LayerMask.NameToLayer("Default");
                character.Rb.gravityScale = character.NormalGravityScale;
            }
        }
    }

    public class GrabbingMovable : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.CurrentMovement.SetGrabbing();
            }
        }
    }

    public class Dashing : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.CurrentMovement.SetDashing();
            }
        }
    }

    public class SittingOnBench : CharacterStateBase { }

    public class Transporting : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.CurrentMovement.SetTransporting();
                character.gameObject.layer = LayerMask.NameToLayer("AheadGround");
                character.Rb.gravityScale = 0;
            } else {
                character.gameObject.layer = LayerMask.NameToLayer("Default");
                character.Rb.gravityScale = character.NormalGravityScale;
            }
        }
    }

    public class BeingBlown : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.Rb.gravityScale = character.NormalGravityScale * character.MovementAttributes.AirHangTimeGravityMultiplier;
            } else {
                character.Rb.gravityScale = character.NormalGravityScale;
            }
        }
    }
}

