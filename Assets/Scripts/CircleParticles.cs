using UnityEngine;

public class CircleParticles : MonoBehaviour
{
    public ParticleSystem circleParticleSystem;
    public int numParticles = 50;
    public float radius = 10f;

    private void Start()
    {
        EmitParticlesInCircle();
    }

    private void EmitParticlesInCircle()
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for (int i = 0; i < numParticles; i++)
        {
            float angle = 360f / numParticles * i;
            Vector3 position = Quaternion.Euler(0, angle, 0) * new Vector3(radius, 0, 0);
            emitParams.position = position;
            circleParticleSystem.Emit(emitParams, 1);
        }
    }
}
