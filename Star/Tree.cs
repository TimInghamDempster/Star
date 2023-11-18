using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Star
{
    internal struct Element
    {
        public float _mass;
        public Vector3 _position;
    }

    internal class Block 
    {
        private Element[,,] _elements;

        private List<Particle> _particles = new();

        internal Block(int fanout)
        {
            _elements = new Element[fanout, fanout, fanout];
        }

        internal Element[,,] Elements => _elements;
        internal List<Particle> Particles => _particles;

        internal Element Average()
        {
            var mass = 0.0f;
            var com = Vector3.Zero;

            for(int x = 0; x < _elements.GetLength(0); x++)
                for (int y = 0; y < _elements.GetLength(1); y++)
                    for (int z = 0; z < _elements.GetLength(2); z++)
                    {
                        var element = _elements[x, y, z];
                        com += element._position;
                        mass += element._mass;
                    }
            

            if (mass != 0.0f)
            {
                com /= mass;
            }

            return new() { _position = com, _mass = mass };
        }
    }

    internal class Layer 
    {
        private readonly Block[,,] _blocks;

        internal Layer(Block[,,] blocks, int blockDim)
        {
            _blocks = blocks;
            BlockDim = blockDim;
        }

        public int BlockDim { get; }

        internal Block[,,] Blocks => _blocks;
    }
    
    public class Tree
    {
        private readonly int _fanout;
        private readonly float _size;
        private readonly float _cellSize;
        private readonly float _elementSize;
        private List<Layer> _layers = new();
        
        public Tree(int layers, int fanout, float size)
        {
            _fanout = fanout;
            _size = size;
            _cellSize = 1.0f / (float)Math.Pow(fanout, layers - 1);
            _elementSize = 1.0f / (float)Math.Pow(fanout, layers);

            for (int l = 0; l < layers; l++)
            {
                var blockDim = (int)Math.Pow(fanout, l);

                var blocks = new Block[blockDim, blockDim, blockDim];

                for(int i = 0; i < blockDim; i++)
                    for (int j = 0; j < blockDim; j++)
                        for (int k = 0; k < blockDim; k++)
                        { 
                            blocks[i, j, k] = new(fanout);
                        }

                var layer = new Layer(blocks, blockDim);
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
            var elmDim = toBuild.BlockDim * _fanout;
            for (int i = 0; i < elmDim; i++)
                for (int j = 0; j < elmDim; j++)
                    for (int k = 0; k < elmDim; k++)
                    {
                        var targetBlock = toBuild.Blocks[i / _fanout, j / _fanout, k / _fanout];
                        var sourceBlock = source.Blocks[i, j, k];
                        targetBlock.Elements[i % _fanout, j % _fanout, k % _fanout] =
                            sourceBlock.Average();
                    }
        }

        private void NormaliseCom(Layer lowestLayer)
        {
            foreach (var block in lowestLayer.Blocks)
            {
                for (int i = 0; i < _fanout; i++)
                    for (int j = 0; j < _fanout; j++)
                        for (int k = 0; k < _fanout; k++)
                        {
                            if(block.Elements[i, j, k]._mass == 0.0f) continue;

                            block.Elements[i, j, k]._position /= block.Elements[i, j, k]._mass;
                        }
            }
        }

        private void AssignParticles(List<Particle> particles, Layer lowestLayer)
        {
            var size = particles.Select(
                p => MathF.Max(MathF.Max(MathF.Abs(p.Position.X), MathF.Abs(p.Position.Y)), MathF.Abs(p.Position.Z))).
                Max()
                * 2.05f;

            foreach (var particle in particles)
            {
                var normalised = (particle.Position + new Vector3(size / 2.05f)) / size;
                var index = normalised / _cellSize;
                var elmIndex = normalised / _elementSize;

                var block = lowestLayer.Blocks[(int)index.X, (int)index.Y, (int)index.Z];

                block.Particles.Add(particle);

                var ex = (int)(elmIndex.X) % _fanout;
                var ey = (int)(elmIndex.Y) % _fanout;
                var ez = (int)(elmIndex.Z) % _fanout;

                block.Elements[ex, ey, ez]._position += particle.Position;
                block.Elements[ex, ey, ez]._mass += 1.0f;
            }
        }

        private void Blank(Layer lowestLayer)
        {
            for(int bi = 0; bi < lowestLayer.Blocks.GetLength(0); bi++) 
                for(int bj = 0; bj < lowestLayer.Blocks.GetLength(1); bj++)
                    for (int bk = 0; bk < lowestLayer.Blocks.GetLength(2); bk++)
                    {
                        var block = lowestLayer.Blocks[bi, bj, bk];
                        block.Particles.Clear();

                        for (int i = 0; i < _fanout; i++)
                            for (int j = 0; j < _fanout; j++)
                                for (int k = 0; k < _fanout; k++)
                                {
                                    block.Elements[i, j, k]._position = Vector3.Zero;
                                    block.Elements[i, j, k]._mass = 0.0f;
                                }
                    }
        }
    }
}
