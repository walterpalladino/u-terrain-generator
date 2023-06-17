using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSTown : MonoBehaviour
{

    [Header("Town Settings")]
    [SerializeField]
    private string placeName;

    [Header("Buildings Settings")]
    [SerializeField]
    private bool buildingsEnabled = true;
    [SerializeField]
    private GameObject[] buildings;
    [SerializeField]
    private int buildingsQty = 6;
    [SerializeField]
    private GameObject buildingsParent;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float buildingsFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float buildingsMaxSlope = 20.0f;
    [SerializeField]
    private float buildingsMinAltitude = 0.0f;
    [SerializeField]
    private float buildingsMaxAltitude = 20.0f;

    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;


    private Vector3 townMin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    private Vector3 townMax = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Generate ()
    {
        Random.InitState(randomSeed);

        Clear();

        PSPlaceholder placeholder = GetComponent<PSPlaceholder>();
        Collider collider = GetComponent<Collider>();

        Vector3 center = collider.bounds.center;
        Vector3 size = collider.bounds.size;
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;

        Debug.Log("Placeholder for Town " + placeName + " located at " + center + " extends " + size + " from " + min + " to " + max);

        townMin = min;
        townMax = max;

        collider.enabled = false;

        PlaceBuildings();

        collider.enabled = true;
    }

    private void PlaceBuildings()
    {
        if (buildingsEnabled)
        {

            int buildingsPlaced = 0;

            for (int n = 0; n < buildingsQty; n++)
            {

                GameObject buildingPrefab = GetBuildingPrefab();
                Vector3 buildingPosition;

                if (GetBuildingPosition(buildingPrefab, buildingsMaxSlope, buildingsMinAltitude, buildingsMaxAltitude, buildingsFreeRadius, out buildingPosition))
                {

                    //GameObject instance = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);

                    GameObject instance = Instantiate(buildingPrefab, buildingPosition, Quaternion.identity);
                    instance.transform.position = buildingPosition;
                    instance.transform.parent = buildingsParent.transform;

                    //float orientation = PickOrientation();
                    //instance.transform.rotation = Quaternion.Euler( new Vector3(instance.transform.rotation.eulerAngles.x, orientation, instance.transform.rotation.eulerAngles.z)); 

                    buildingsPlaced++;
                }
                else
                {
                    //DestroyImmediate(instance);
                }

            }
            Debug.Log("Placed " + buildingsPlaced + " of " + buildingsQty + " buildings.");
            
        }

    }

    private float PickOrientation()
    {
        return Random.Range(1, 4) * 90.0f;
    }
    /*
    private bool GetItemPosition(float maxSlope, float minAltitude, float maxAltitude, float freeRadius, out Vector3 position)
    {
        //  Obtain a position
        position = new Vector3(Random.Range(townMin.x, townMax.x), 0, Random.Range(townMin.z, townMax.z));

        float height;
        Vector3 normal;

        PSHit psHit = CheckAt(position, out height, out normal);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != PSHit.TERRAIN_HIT)
        {
            return false;
        }
        position.y = height;

        //  Check Min and Max Altitude
        if (height < minAltitude || height > maxAltitude)
        {
            return false;
        }
        
                if (CheckPlaceholderAt(position))
                {
                    return false;
                }
        
        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        //  Return the position
        return true;
    }*/

    private bool GetBuildingPosition(GameObject prefab, float maxSlope, float minAltitude, float maxAltitude, float freeRadius, out Vector3 position)
    {
        
        Vector3 centerPrefab = prefab.gameObject.transform.position;
        Vector3 extentsPrefab = prefab.gameObject.GetComponent<PSBuilding>().size / 2;  //  extents == size / 2

        //Debug.Log(centerPrefab);
        //Debug.Log(extentsPrefab);

        //  Obtain a position
        position = new Vector3(Random.Range(townMin.x + extentsPrefab.x, townMax.x - extentsPrefab.x), 0, Random.Range(townMin.z + extentsPrefab.z, townMax.z - extentsPrefab.z));

        //Debug.Log("Generated position : " + position);
        //Debug.Log("Extents Prefab : " + extentsPrefab);

        float height;
        Vector3 normal;

        PSHit psHit = CheckAt(position, out height, out normal);
        //Debug.Log("Height : " + height);
        position.y = height;


        //  Check overlap
        /*
        Collider[] hitColliders = Physics.OverlapBox(position, extentsPrefab, Quaternion.identity);
        if (hitColliders.Length > 0)
        {
            Debug.Log("Hit : " + hitColliders.Length + " colliders.");
            bool hit = false;
            int i = 0;
            while (i < hitColliders.Length)
            {
                //Output all of the collider names
                Debug.Log("Hit : " + hitColliders[i].name + i);

                if (hitColliders[i].gameObject.GetComponent<PSTerrain>() == null)
                {
                    hit = true;
                }

                //Increase the number of Colliders in the array
                i++;
            }

            //            continue;
            if (hit)
                return false;
        }
        */
        /*
        RaycastHit[] hits = Physics.BoxCastAll(position + Vector3.up * 1000, extentsPrefab, Vector3.down);
        if (hits.Length > 0)
        {
            Debug.Log("Hit : " + hits.Length + " colliders.");
            bool hit = false;
            int i = 0;
            while (i < hits.Length)
            {
                //Output all of the collider names
                Debug.Log("Hit : " + hits[i].collider.name + i);

                if (hits[i].collider.gameObject.GetComponent<PSTerrain>() == null)
                {
                    hit = true;
                }

                //Increase the number of Colliders in the array
                i++;
            }

            //            continue;
            if (hit)
                return false;
        }
        */

        //  Check Overlap with Other Buildings
        PSBuilding[] buildings = FindObjectsOfType<PSBuilding>();
        Debug.Log("Buildings found : " + buildings.Length);

        for (int n = 0; n < buildings.Length; n++)
        {
            Bounds bounds = new Bounds(buildings[n].gameObject.transform.position, buildings[n].size);
            /*
            Vector3 center = collider.bounds.center;
            Vector3 extents = collider.bounds.extents;  //  extents == size / 2

            Debug.Log("Checking collider at position : " + center);
            Debug.Log("Extending : " + extents);
            */
            Bounds newBounds = new Bounds(position, prefab.gameObject.GetComponent<PSBuilding>().size);
            Debug.Log(bounds);
            Debug.Log(newBounds);
            if (bounds.Intersects(newBounds))
            {
                return false;
            }
            
        }

        return true;
        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != PSHit.TERRAIN_HIT)
        {
            return false;
        }

        //  Check Min and Max Altitude
        if (height < minAltitude || height > maxAltitude)
        {
            return false;
        }
        /*
                if (CheckPlaceholderAt(position))
                {
                    return false;
                }
        */
        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        //  Return the position
        return true;
    }

    private PSHit CheckAt(Vector3 position, out float height, out Vector3 normal)
    {

        height = -1;
        normal = Vector3.zero;

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.GetComponent<PSPlaceholder>() != null)
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.PLACEHOLDER_HIT;
            }
            else
            if (hit.collider.gameObject.GetComponent<PSTerrain>() != null)
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.TERRAIN_HIT;
            }
            else
            {
                return PSHit.UNKNOWN_HIT;
            }
        }

        return PSHit.NO_HIT;
    }

    private GameObject GetBuildingPrefab()
    {
        int idx = Random.Range(0, buildings.Length);
        return buildings[idx];
    }

    private void CheckParent ()
    {
        if (buildingsParent == null)
        {
            buildingsParent = new GameObject();
            //buildingsParent.transform.position = this.transform.position;
            //buildingsParent.transform.rotation = this.transform.rotation;
            buildingsParent.name = placeName;
        }
    }

    public void Clear()
    {

        CheckParent();

        if (Application.isPlaying)
        {

            foreach (Transform child in buildingsParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }


        }
        else
        {

            while (buildingsParent.transform.childCount != 0)
            {
                DestroyImmediate(buildingsParent.transform.GetChild(0).gameObject);
            }

        }
    }

}
