using FluentAssertions;
using Microsoft.Xna.Framework;
using Star;

namespace StarTests
{
    [TestClass]
    public class TestTree
    {
        [TestMethod]
        public void EvenDistributionReportedAtTopLevel()
        {
            var fanout = 4;
            var tree = new Tree(3, fanout);
            var particles = new List<Particle>();

            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                    for (int k = 0; k < 200; k++)
                    {
                        var particle = new Particle() { Position = new Vector3(i, j, k) };
                        particles.Add(particle);
                    }

            tree.Build(particles);

            var testMass = tree.GetElement(0, 0, 0, 0)._mass;

            for (int i = 0; i < fanout; i++)
                for (int j = 0; j < fanout; j++)
                    for (int k = 0; k < fanout; k++)
                    {
                        var element = tree.GetElement(i, j, k, 0);
                        var elementMass = element._mass;
                        elementMass.Should().BeGreaterThan(testMass * 0.9f);
                        elementMass.Should().BeLessThan(testMass * 1.1f);
                    }
        }

    }
}