using Microsoft.Xna.Framework;
using MultiplayerShooter.Client.FSM;
using MultiplayerShooter.Client.Managers;
using Nez;

namespace MultiplayerShooter.Client.Components.Player
{
    public class PlayerState : State<PlayerState, PlayerComponent>
    {
        protected InputManager _input => Core.getGlobalManager<InputManager>();

        public override void begin() { }

        public override void end() { }

        public void handleInput()
        {
            if (isMovementAvailable())
            {
                if (entity.isOnGround() && isMovementAvailable() && _input.JumpButton.isPressed)
                {
                    fsm.resetStackTo(new JumpingState(true));
                }

                if (_input.AttackButton.isPressed)
                {
                    fsm.pushState(new ShotingState());
                }
            }
        }

        protected bool isMovementAvailable()
        {
            return Core.getGlobalManager<InputManager>().isMovementAvailable();
        }

        public override void update()
        {
            handleInput();
        }
    }

    public class StandState : PlayerState
    {
        public override void begin()
        {
            entity.SetAnimation(PlayerComponent.Animations.Stand);
        }

        public override void update()
        {
            base.update();

            if (!entity.isOnGround())
            {
                fsm.changeState(new JumpingState(false));
                return;
            }

            if (entity.isOnGround())
            {
                if (entity.Velocity.X > 0 || entity.Velocity.X < 0)
                {
                    entity.SetAnimation(PlayerComponent.Animations.Stand);
                }
                else
                {
                    entity.SetAnimation(PlayerComponent.Animations.Stand);
                }
            }
        }
    }

    public class JumpingState : PlayerState
    {
        private bool _needJump;

        public JumpingState(bool needJump)
        {
            _needJump = needJump;
        }

        public override void begin()
        {
            if (_needJump)
            {
                _needJump = false;
                entity.Jump();
            }
        }

        public override void update()
        {
            base.update();

            if (entity.isOnGround())
            {
                fsm.resetStackTo(new StandState());
            }
        }
    }

    public class ShotingState : PlayerState
    {
        private bool _shot;

        public override void begin()
        {
            entity.SetAnimation(PlayerComponent.Animations.Shot);
        }

        public override void update()
        {
            if (!_shot)
            {
                _shot = true;
                shot();
            }
            if (entity.sprite.Looped)
            {
                fsm.popState();
            }
        }

        private void shot()
        {
            entity.shoot();
            /*
            var start = entity.entity.position;
            var end = start + 200 * Vector2.UnitX;
            entity.createEntityOnMap()
                .addComponent(new HitscanComponent(start, end, false));
                */
        }
    }
}
