using UnityEngine;
using Mirror;

public class Gun : NetworkBehaviour
{
    [SerializeField] private Camera fpsCam;
    [SerializeField] private GameObject gun;

    [SerializeField] private float damage = 15f;
    [SerializeField] private float range = 1000f;
    [SerializeField] private float fireRate = 6f;
    [SerializeField] private float nextToFire = 0f;

    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private float impactForce = 10f;
    [SerializeField] private float upRecoil = -1f;

    private AudioSource uspShot;

    private void Awake()
    {
        uspShot = gun.GetComponent<AudioSource>();
    }

    private void Start()
    {

    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextToFire)
        {
            nextToFire = Time.time + 1f / fireRate;
            Shoot();
            uspShot.Play();
            CameraFP.xRotation += upRecoil;
        }
        gun.transform.transform.localRotation = Quaternion.Euler(CameraFP.xRotation, 0f, 0f);
    }

    public void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null && hit.rigidbody.CompareTag("Enemy"))
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }  
    }
}
