using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Star
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
    }

    internal class Physics
    {
        const float G = 0.01f;
        public static void Step(List<Particle> particles, Tree tree)
        {
            tree.Build(particles);

            foreach(var particle in particles)
            {
                particle.Position += particle.Velocity;

                foreach(var particle2 in particles)
                {
                    if (particle == particle2) continue;

                    var delta = particle2.Position - particle.Position;
                    var distSq = delta.LengthSquared();
                    delta.Normalize();

                    particle.Velocity += delta * G / distSq;
                }
            }
        }
    }
}
