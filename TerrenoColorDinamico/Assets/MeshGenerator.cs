using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;

    //Array que recoge los colores de cada vertice
    Color[] colors;

    //Arrays donde guardaremos los vertices y los usaremos en triangles para rellenar los cuadrados a base de triangulos. (2 por cada cuadrado)
    Vector3[] vertices;
    int[] triangles;

    //Ancho y alto de la malla a crear
    public int xSize = 20;
    public int zSize = 20;

    //Gradiente de colores.
    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        /*Creamos un array vertices con el tamaño adecuado 
        (cada 3 cuadrados necesitamos 4 vertices "._._._." de ahí el más 1 siempre al tamaño)*/
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //En este bucle pasamos por todos los vertices y creamos la forma mediante la formula con PerlinNoise.
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2.5f;

                vertices[i] = new Vector3(x, y, z);

                if (y < minTerrainHeight)
                {
                    minTerrainHeight = y;
                }
                if (y > maxTerrainHeight)
                {
                    maxTerrainHeight = y;
                }
                i++;
            }
        }


        //Aquí creamos el array que recogerá todos los vertices de cada triangulo, por cada cuadrado (xSize * zSize) son 6 vertices ya que hay que poner 2 triangulos.
        triangles = new int[xSize * zSize * 6];
        
        //Variables para ir recorriendo los vertices.
        int vert = 0;
        int tris = 0;

        //Bucle que va creando los triangulos, tener en cuenta que es necesario que se creen en el sentido de las agujas del reloj, o no funciona.
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        //Array de colores para cada vertice.
        colors = new Color[vertices.Length];

        //Recorremos todos los vertices y le ponemos el color.
        for (int i = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                /*Esta función sirve para sacar un valor entre 1 y 0
                 Usamos el máximo y minimo de altura, y con eso hacemos la escala.*/
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);

                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
    }
}
