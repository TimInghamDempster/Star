using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Star
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
    }

    internal class Physics
    {
        const float G = 0.00001f;
        public static void Step(List<Particle> particles, Tree tree)
        {
            tree.Build(particles);

            Parallel.ForEach(particles, particle =>
            {
                particle.Position += particle.Velocity;

                /*foreach(var particle2 in particles)
                {
                    if (particle == particle2) continue;

                    var delta = particle2.Position - particle.Position;
                    var distSq = delta.LengthSquared();
                    delta.Normalize();

                    particle.Velocity += delta * G / distSq;
                }*/
                foreach (var cell in tree.UpperCells)
                {
                    var delta = cell._position - particle.Position;
                    var distSq = delta.LengthSquared();
                    delta.Normalize();

                    // Only one element in the cell, which is the particle itself
                    if (distSq == 0) { continue; }

                    particle.Velocity += (delta * G * cell._mass) / distSq;
                }
            });
        }
    }
}
