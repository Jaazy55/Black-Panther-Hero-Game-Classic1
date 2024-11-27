using System;

namespace Game.Character.CameraEffects
{
	public class Spring
	{
		public void Setup(float mass, float distance, float springStrength, float damping)
		{
			this.mass = mass;
			this.distance = distance;
			this.springConstant = springStrength;
			this.damping = damping;
			this.velocity = 0f;
		}

		public void AddForce(float force)
		{
			this.velocity += force;
		}

		public float Calculate(float timeStep)
		{
			this.springForce = -this.springConstant * this.distance - this.velocity * this.damping;
			this.acceleration = this.springForce / this.mass;
			this.velocity += this.acceleration * timeStep;
			this.distance += this.velocity * timeStep;
			return this.distance;
		}

		private float mass;

		private float distance;

		private float springConstant;

		private float damping;

		private float acceleration;

		private float velocity;

		private float springForce;
	}
}
