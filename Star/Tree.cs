using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Star
{
    public struct Element
    {
        public float _mass;
        public Vector3 _position;
    }

    internal class Layer 
    {
        private readonly Element[,,] _elements;

        internal Layer(Element[,,] elements)
        {
            _elements = elements;
        }

        internal Element[,,] Elements => _elements;
    }
    
    public class Tree
    {
        private List<Layer> _layers = new();

        public Element GetElement(int x, int y, int z, int layer)
        {
            return _layers[layer].Elements[x, y, z];
        }

        internal IEnumerable<Element> UpperCells
        {
            get
            {
                var size = _layers.Last().Elements.GetLength(0);
                var layer = _layers.Last();

                for(int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        for (int k = 0; k < size; k++)
                        {
                            yield return layer.Elements[i, j, k];
                        }
            }
        }

        public Tree(int layers, int fanout)
        {
            for (int l = 1; l < layers + 1; l++)
            {
                var dim = (int)Math.Pow(fanout, l);

                var layer = new Layer(new Element[dim,dim,dim]);
                _layers.Add(layer);
            }
        }

        public void Build(List<Particle> particles)
        {
            var lowestLayer = _layers.Last();

            Blank(lowestLayer);
            AssignParticles(particles, lowestLayer);
            NormaliseCom(lowestLayer);

            for(int l = _layers.Count - 1; l > 0; l--)
            {
                BuildFromLower(_layers[l - 1], _layers[l]);
            }
        }

        private void BuildFromLower(Layer toBuild, Layer source)
        {
            var targetDim = toBuild.Elements.GetLength(0);
            var sourceDim = source.Elements.GetLength(0);
            var elmDim = sourceDim / targetDim;

            for (int i = 0; i < sourceDim; i++)
                for (int j = 0; j < sourceDim; j++)
                    for (int k = 0; k < sourceDim; k++)
                    {
                        var targetBlock = toBuild.Elements[i / elmDim, j / elmDim, k / elmDim];
                        var sourceBlock = source.Elements[i, j, k];

                        toBuild.Elements[i / elmDim, j / elmDim, k / elmDim]._mass += sourceBlock._mass;
                        toBuild.Elements[i / elmDim, j / elmDim, k / elmDim]._position += sourceBlock._position;
                    }

            var blockScale = elmDim * elmDim * elmDim;

            for (int i = 0; i < targetDim; i++)
                for (int j = 0; j < targetDim; j++)
                    for (int k = 0; k < targetDim; k++)
                    {
                        toBuild.Elements[i, j, k]._position /= blockScale;
                    }
        }

        private void NormaliseCom(Layer lowestLayer)
        {
            var size = lowestLayer.Elements.GetLength(0);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 0; k < size; k++)
                    {
                        if (lowestLayer.Elements[i, j, k]._mass == 0.0f) continue;

                        lowestLayer.Elements[i, j, k]._position /= lowestLayer.Elements[i, j, k]._mass;
                    }
        }

        private void AssignParticles(List<Particle> particles, Layer lowestLayer)
        {
            var size = particles.Select(
                p => MathF.Max(MathF.Max(MathF.Abs(p.Position.X), MathF.Abs(p.Position.Y)), MathF.Abs(p.Position.Z))).
                Max()
                * 2.01f;

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var particle in particles)
            {
                min = new Vector3(MathF.Min(min.X, particle.Position.X), MathF.Min(min.Y, particle.Position.Y), MathF.Min(min.Z, particle.Position.Z));
                max = new Vector3(MathF.Max(max.X, particle.Position.X), MathF.Max(max.Y, particle.Position.Y), MathF.Max(max.Z, particle.Position.Z));
            }

            min *= 0.999f;
            max *= 1.001f;
            var range = max - min;

            var normalisedElementSize = 1.0f / lowestLayer.Elements.GetLength(0);

            foreach (var particle in particles)
            {
                var normalised = (particle.Position - min) / range;
                var elmIndex = normalised / normalisedElementSize;

                var ex = (int)(elmIndex.X);
                var ey = (int)(elmIndex.Y);
                var ez = (int)(elmIndex.Z);

                lowestLayer.Elements[ex, ey, ez]._position += particle.Position;
                lowestLayer.Elements[ex, ey, ez]._mass += 1.0f;
            }
        }

        private void Blank(Layer lowestLayer)
        {
            for (int i = 0; i < lowestLayer.Elements.GetLength(0); i++)
                for (int j = 0; j < lowestLayer.Elements.GetLength(1); j++)
                    for (int k = 0; k < lowestLayer.Elements.GetLength(2); k++)
                    {

                        lowestLayer.Elements[i, j, k]._position = Vector3.Zero;
                        lowestLayer.Elements[i, j, k]._mass = 0.0f;

                    }
        }
    }
}
