using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterState {
    public interface ICharacterState {
        void HandleStateChange(Character character, bool isActive);
    }

    public abstract class CharacterStateBase : ICharacterState {
        public virtual void HandleStateChange(Character character, bool isActive) { }
    }

    public class Free : CharacterStateBase { }

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
                character.CurrentSpeed = character.GrabbingStoneSpeed;
            } else {
                character.CurrentSpeed = character.NormalMoveSpeed;
            }
        }
    }

    public class Dashing : CharacterStateBase { }

    public class SittingOnBench : CharacterStateBase { }

    public class Transporting : CharacterStateBase {
        public override void HandleStateChange(Character character, bool isActive) {
            if (isActive) {
                character.gameObject.layer = LayerMask.NameToLayer("AheadGround");
                character.Rb.gravityScale = 0;
            } else {
                character.gameObject.layer = LayerMask.NameToLayer("Default");
                character.Rb.gravityScale = character.NormalGravityScale;
            }
        }
    }
}

