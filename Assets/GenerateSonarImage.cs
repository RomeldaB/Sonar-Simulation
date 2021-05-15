using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public class GenerateSonarImage : MonoBehaviour
{
    private Camera camera;
    private int image_width;
    private int image_height;
    private int intercept;
    private Texture2D image;
    // Initial angle and range values
    public float range = 3;
    public float angle = 60;
    void Start()
    {
        camera = this.gameObject.GetComponent<Camera>();
        image = new Texture2D(1000, 1000, TextureFormat.RGBA32, false);
        print("Start");
    }

    void Update()
    {
        image_width = (int)(range * 250);
        image_height = (int)(2 * image_width * Mathf.Sin(Utils.DegreeToRadian(angle/2)));
        intercept = (int)(image_width * Mathf.Cos(Utils.DegreeToRadian(angle/2)));

        if (Input.GetKeyDown("space"))
        {
            image = new Texture2D(image_width, image_height, TextureFormat.RGBA32, false);
            SetOriginalPixels();
            RayTracing();
            GenerateImage(image, "./sonar.png");
            // Perlin noise addition for different intensity values
            AddNoise(image_width*image_height, "./sonar_noise.png");
            AddNoise(image_width*image_height/4, "./sonar_noise_2.png");
            AddNoise(image_width*image_height/10, "./sonar_noise_3.png");
            print("Done!");
        }
    }

    // Initialize the range and angle sliders on GUI
    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), "Range: " + range);
        // The range is set to be between 0.5 and 5
        range = GUI.HorizontalSlider(new Rect(Screen.width - 90, 20, 80, 20), range, 0.5f, 5.0f);
        range = Mathf.Round(range * 100f) / 100f;
        GUI.Box(new Rect(Screen.width - 100, 50, 100, 50), "Angle: " + angle);
        // The angle is set to be between 30 and 100
        angle = GUI.HorizontalSlider(new Rect(Screen.width - 90, 70, 80, 20), angle, 30, 100);
        angle = Mathf.Round(angle);
        GUI.DrawTexture(new Rect(10, 10, 300, 300), image, ScaleMode.ScaleToFit, true);
    }

    void RayTracing()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        // Initialize the orienting ray direction
        Vector3 origin = camera.transform.position;
        Vector3 rotation = Quaternion.AngleAxis(angle / 2, camera.transform.up) * camera.transform.forward;
        // Fixed value of vertical angle set to 18
        float vertical_angle = 18;
        for (float v = -vertical_angle/2; v <= vertical_angle/2; v += 0.1f)
        {
            for (float i = 0; i <= angle; i += 0.1f)
            {
                // Calculate the direction vector of each ray
                Vector3 direction = Quaternion.AngleAxis(-i, camera.transform.up) * rotation;
                // Calculate the height difference between rays in different planes along the vertical angle
                direction.y += Mathf.Tan(Utils.DegreeToRadian(v))*range;
                direction = Vector3.Normalize(direction);
                // Initialize ray
                Ray ray = new Ray(origin, direction);
                RaycastHit hit;

                // Initialize default values for visualization
                Vector3 position = ray.GetPoint(range);
                float ray_length = range;
                Color pixel_color = Color.black;

                if (Physics.Raycast(ray, out hit, range, layerMask))
                {
                    // Get the information for the hit point in collider
                    position = hit.point;
                    ray_length = hit.distance;
                    string name = hit.collider.name;

                    // Calculate color of specific pixel in the image using backscatter
                    float backscatter = Utils.calculateBackscatter(name, direction, hit.normal);
                    pixel_color = new Color(backscatter, backscatter, backscatter);
                }
                // Map points into the texture consider the two cases for angle smaller or bigger than horizontal angle
                if (i <= angle / 2)
                {
                    int x = (int)(Mathf.Cos(Utils.DegreeToRadian(angle / 2 - i)) * ray_length * 250);
                    int y = (int)(Mathf.Sin(Utils.DegreeToRadian(angle / 2 - i)) * ray_length * 250);
                    image.SetPixel(x, image_height / 2 - y, pixel_color);
                }
                else
                {
                    int x = (int)(Mathf.Cos(Mathf.PI / 180 * (i - angle / 2)) * ray_length * 250);
                    int y = (int)(Mathf.Sin(Mathf.PI / 180 * (i - angle / 2)) * ray_length * 250);
                    image.SetPixel(x, image_height / 2 + y, pixel_color);
                }

                /* Uncomment the line below in order to visualize the rays (commented due to the increase in time)
                Suggestion: Change the vertical angle interval to v += 3 or more to see the visualization clearly */
                
                // VisualizeRay(ray.origin, position, Color.white);
            }
        }
    }
    
    // Check whether the point is inside the sonar shape in the texture
    bool isPointInsideShape(int x, int y) {
        bool isInLowerTriangle = (image_height * x + 2 * intercept * y) <= intercept * image_height;
        bool isInUpperTriangle = (2 * y * intercept - intercept * image_height) >= x * image_height;
        bool isPointInCircle = (Mathf.Pow(x, 2) + Mathf.Pow((y - image_height/2), 2)) <= Mathf.Pow(image_width, 2);
        return isPointInCircle && !isInLowerTriangle && !isInUpperTriangle;
    }

    // Set the original grey and black pixels
    void SetOriginalPixels()
    {
        // Set the scene grey and the outer part to black
        for (int i = 0; i < image_width; i++)
        {
            for (int j = 0; j < image_height; j++)
            {
                if (isPointInsideShape(i, j)){
                    image.SetPixel(i, j, Color.black);
                }
                else
                    image.SetPixel(i, j, new Color(0.2f, 0.2f, 0.2f));
            }
        }
    }

    // Function to visualize the cast rays
    void VisualizeRay(Vector3 start, Vector3 end, Color col)
    {
        GameObject newObject = new GameObject();
        LineRenderer line = newObject.AddComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        line.startColor = col;
        line.endColor = col;
        line.startWidth = 0.0001f;
        line.endWidth = 0.0001f;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.useWorldSpace = false;
    }
    
    // Generate the texture in the given path
    void GenerateImage(Texture2D texture, string path)
    {
        texture.Apply();
        var pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
        }
    }

    // Addition of perlin noise
    void AddNoise(int intensity, string fileName)
    {
        Texture2D texture = new Texture2D(image_width, image_height, TextureFormat.RGBA32, false);
        texture.SetPixels(image.GetPixels());
        for (int i = 0; i < intensity; i++)
        {
            int x = Random.Range(0, image_width);
            int y = Random.Range(0, image_height);

            float sample = Mathf.PerlinNoise(x, y);
            if (isPointInsideShape(x, y))
                texture.SetPixel(x, y, new Color(sample, sample, sample));
        }
        GenerateImage(texture, fileName);
    }
}
