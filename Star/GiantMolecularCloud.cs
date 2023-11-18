using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Star
{
    internal class GiantMolecularCloud
    {
        const float _diameterInLightyears = 100.0f;
        const float _radiusInLightyears = _diameterInLightyears / 2.0f;
        const int _particleCount = 1000;
        private readonly float _screenHeight;
        const float _initialParticleSpeed = 0.00f;

        public GiantMolecularCloud(float screenHeight)
        {
            var random = new Random();
            var centre = new Vector3(_radiusInLightyears);
            var p = () => (float)random.NextDouble() * _diameterInLightyears;
            var v = () => ((float)random.NextDouble() - 0.5f) * _initialParticleSpeed;
            
            for (int i = 0; i < _particleCount; i++)
            {
                Vector3 pos;
                do
                {
                    pos = new Vector3(p(), p(), p());
                } while (Vector3.Distance(pos, centre) > _radiusInLightyears);

                Particles.Add(new()
                {
                    Position = pos,
                    Velocity = new Vector3(v(), v(), v())
                });
            }

            _screenHeight = screenHeight;
        }

        public List<Particle> Particles { get; } = new(_particleCount);

        public float DiameterInLightYears => _diameterInLightyears;

        public IEnumerable<Vector3> Points => 
            Particles.Select(p => p.Position / _diameterInLightyears * _screenHeight);
    }
}
