using NUnit.Framework;
using PlagueHunter.Combat;
using UnityEngine;

namespace PlagueHunter.Tests
{
    public sealed class HealthTests
    {
        private GameObject _gameObject;
        private Health _health;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("HealthTarget");
            _health = _gameObject.AddComponent<Health>();
            _health.ResetHealth();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Reset_SetsHealthToMax()
        {
            Assert.AreEqual(_health.Max, _health.Current);
            Assert.IsFalse(_health.IsDead);
        }

        [Test]
        public void TakeDamage_ReducesCurrent()
        {
            _health.TakeDamage(30f);

            Assert.AreEqual(_health.Max - 30f, _health.Current);
        }

        [Test]
        public void TakeDamage_ClampsAtZero()
        {
            _health.TakeDamage(_health.Max * 2f);

            Assert.AreEqual(0f, _health.Current);
            Assert.IsTrue(_health.IsDead);
        }

        [Test]
        public void TakeDamage_IgnoresNonPositiveAmount()
        {
            _health.TakeDamage(0f);
            _health.TakeDamage(-50f);

            Assert.AreEqual(_health.Max, _health.Current);
        }

        [Test]
        public void TakeDamage_IgnoredWhenInvulnerable()
        {
            _health.Invulnerable = true;
            _health.TakeDamage(40f);

            Assert.AreEqual(_health.Max, _health.Current);
        }

        [Test]
        public void TakeDamage_AppliedAfterInvulnerabilityEnds()
        {
            _health.Invulnerable = true;
            _health.TakeDamage(40f);

            _health.Invulnerable = false;
            _health.TakeDamage(40f);

            Assert.AreEqual(_health.Max - 40f, _health.Current);
        }

        [Test]
        public void Died_RaisedOnce()
        {
            int calls = 0;
            _health.Died += () => calls++;

            _health.TakeDamage(_health.Max);
            _health.TakeDamage(_health.Max);

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void Changed_ReportsNormalizedValue()
        {
            float last = -1f;
            _health.Changed += value => last = value;

            _health.TakeDamage(_health.Max * 0.25f);

            Assert.AreEqual(0.75f, last, 0.0001f);
        }
    }
}