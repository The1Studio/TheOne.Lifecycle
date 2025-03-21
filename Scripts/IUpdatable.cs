﻿#nullable enable
namespace TheOne.Lifecycle
{
    public interface IUpdatable
    {
        public void Update();
    }

    public interface ILateUpdatable
    {
        public void LateUpdate();
    }

    public interface IFixedUpdatable
    {
        public void FixedUpdate();
    }
}