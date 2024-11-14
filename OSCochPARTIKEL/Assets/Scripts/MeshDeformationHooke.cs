using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformationHooke : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;
    private Vector3[] vertexVelocities;  // Håller koll på hastighet för varje vertex
    private float springConstant = 50f;  // Fjäderkonstanten för Hookes lag
    private float damping = 0.98f;       // Dämpningsfaktor för att stabilisera rörelsen
    private float restLength = 0.1f;     // Vilolängd för fjädrarna mellan närliggande vertexer

    private Vector3 gravity = new Vector3(0, -9.81f, 0);  // Gravitationskraft nedåt

    void Start()
    {
        // Hämta meshen från MeshFilter-komponenten
        mesh = GetComponent<MeshFilter>().mesh;

        // Kopiera ursprungliga vertex-positioner för att referera till senare
        originalVertices = mesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length];

        // Kopiera ursprungliga vertex-positioner till displacedVertices
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
    }

    void Update()
    {
        ApplyGravity();
        ApplySpringForces();
        UpdateMesh();
    }

    private void ApplyGravity()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            // Applicera gravitationen på varje vertex
            vertexVelocities[i] += gravity * Time.deltaTime;
        }
    }

    private void ApplySpringForces()
    {
        int width = Mathf.RoundToInt(Mathf.Sqrt(originalVertices.Length));  // Bredden på vertex-matrisen (antag ett kvadratiskt plan)
        
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            Vector3 force = Vector3.zero;

            // Kolla närliggande vertexer i en 2D-grid
            int x = i % width;
            int y = i / width;

            // Skapa fjädrar till närmaste grannar (vänster, höger, ovan, nedan och diagonalt)
            force += GetSpringForce(i, x + 1, y, width);     // Höger
            force += GetSpringForce(i, x - 1, y, width);     // Vänster
            force += GetSpringForce(i, x, y + 1, width);     // Ovan
            force += GetSpringForce(i, x, y - 1, width);     // Nedan
            force += GetSpringForce(i, x + 1, y + 1, width); // Diagonal up-right
            force += GetSpringForce(i, x - 1, y + 1, width); // Diagonal up-left
            force += GetSpringForce(i, x + 1, y - 1, width); // Diagonal down-right
            force += GetSpringForce(i, x - 1, y - 1, width); // Diagonal down-left

            // Uppdatera hastighet baserat på kraft
            vertexVelocities[i] += force * Time.deltaTime;
            vertexVelocities[i] *= damping;  // Dämpning för stabilisering

            // Uppdatera vertex-position baserat på hastighet
            displacedVertices[i] += vertexVelocities[i] * Time.deltaTime;
        }
    }

    private Vector3 GetSpringForce(int index, int neighborX, int neighborY, int width)
    {
        // Kontrollera att grannarna är inom bounds
        if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= width)
        {
            return Vector3.zero;
        }

        int neighborIndex = neighborY * width + neighborX;

        Vector3 displacement = displacedVertices[neighborIndex] - displacedVertices[index];
        float distance = displacement.magnitude;

        // Beräkna Hookes fjäderkraft: F = -k * (x - x_rest)
        float springForceMagnitude = -springConstant * (distance - restLength);

        // Normalisera displacement och multiplicera med fjäderkraften
        Vector3 springForce = springForceMagnitude * displacement.normalized;

        return springForce;
    }

    private void UpdateMesh()
    {
        // Uppdatera vertex-positioner på meshen
        mesh.vertices = displacedVertices;
        mesh.RecalculateNormals();
    }
}
