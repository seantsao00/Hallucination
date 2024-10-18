using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class CharacterState {
    public interface ICharacterState {
        void HandleStateChange(bool isActive);
    }

    public abstract class CharacterStateBase : ICharacterState {
        protected Character character;
        protected CharacterStateBase(Character character) { this.character = character; }
        public virtual void HandleStateChange(bool isActive) {}
    }

    public class Free : CharacterStateBase {
        public Free(Character character) : base(character) {}
    }

    public class Climbing : CharacterStateBase {
        public Climbing(Character character) : base(character) {}

        public override void HandleStateChange(bool isActive) {
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
        public GrabbingMovable(Character character) : base(character) {}
    }

    public class Dashing : CharacterStateBase {
        public Dashing(Character character) : base(character) {}
    }

    public class SittingOnBench : CharacterStateBase {
        public SittingOnBench(Character character) : base(character) {}
    }

    public class Transporting : CharacterStateBase {
        public Transporting(Character character) : base(character) {}

        public override void HandleStateChange(bool isActive) {
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

