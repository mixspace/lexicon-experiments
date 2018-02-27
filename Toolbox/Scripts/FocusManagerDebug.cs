// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using Mixspace.Lexicon;
using UnityEngine;

namespace Mixspace.Lexicon.Samples
{
    public class FocusManagerDebug : MonoBehaviour
    {
        public int interval = 50; // ms

        public Material particleMaterial;
        public Color startColor = Color.white;
        public Color endColor = Color.white;
        public Color dwellColor = Color.magenta;
        public float particleSize = 0.02f;

        private LexiconFocusManager focusManager;

        private ParticleSystem _particleSystem;
        private ParticleSystem.Particle[] particles;

        void Start()
        {
            focusManager = LexiconFocusManager.Instance;

            int markerCount = (int)(focusManager.BufferLength * 1000 / interval);

            if (_particleSystem == null)
            {
                _particleSystem = gameObject.AddComponent<ParticleSystem>();
                gameObject.GetComponent<ParticleSystemRenderer>().material = particleMaterial;

                var main = _particleSystem.main;
                main.maxParticles = markerCount;
                main.startSize = particleSize;
                main.startLifetime = float.PositiveInfinity;

                var emission = _particleSystem.emission;
                emission.enabled = false;

                var shape = _particleSystem.shape;
                shape.enabled = false;

                _particleSystem.Emit(markerCount);
            }

            if (particles == null || particles.Length < _particleSystem.main.maxParticles)
                particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];

            int numParticlesAlive = _particleSystem.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                float t = i / ((float)particles.Length);
                particles[i].startColor = Color.Lerp(startColor, endColor, t);
            }

            _particleSystem.SetParticles(particles, numParticlesAlive);
        }

        void LateUpdate()
        {
            float startTime = Time.realtimeSinceStartup;

            float seconds = (int)startTime;
            int milliseconds = (int)((startTime - seconds) * 1000);
            int chunks = milliseconds / interval;
            milliseconds = chunks * interval;
            startTime = seconds + milliseconds / 1000.0f;

            int timeOffset = 0;

            int numParticlesAlive = _particleSystem.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                timeOffset += interval;
                float time = startTime - timeOffset / 1000.0f;

                FocusDwellPosition dwellPosition = focusManager.GetFocusData<FocusDwellPosition>(time, 0.1f);
                FocusPosition focusPosition = focusManager.GetFocusData<FocusPosition>(time);
                if (dwellPosition != null)
                {
                    particles[i].position = dwellPosition.Position;
                    particles[i].startColor = dwellColor;
                }
                else if (focusPosition != null)
                {
                    particles[i].position = focusPosition.Position;
                    float t = i / ((float)particles.Length);
                    particles[i].startColor = Color.Lerp(startColor, endColor, t);
                }
                else
                {
                    particles[i].position = new Vector3(1000, 1000, 1000);
                }
            }

            _particleSystem.SetParticles(particles, numParticlesAlive);
        }
    }
}
