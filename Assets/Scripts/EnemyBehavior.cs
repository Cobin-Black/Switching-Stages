using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;

    private Rigidbody2D enemyRigidBody;

    private Transform currentPosition;

    [SerializeField] private float enemySpeed;

    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();

        currentPosition = pointB.transform;
    }

    void Update()
    {
        GroundedEnemyMovement();
        AirEnemyMovement();
        EnemyPosition();
    }

    #region Different Enemy Types Movement
    //Gets the grounded enemy movment
    void GroundedEnemyMovement()
    {
        Vector2 point = currentPosition.position - transform.position;

        //Checks what point it need to go to next and makes it move in said direction
        if(currentPosition == pointB.transform && tag == "Grounded Enemy")
        {
            enemyRigidBody.velocity = new Vector2(enemySpeed, 0);
        }
        else if(currentPosition == pointA.transform && tag == "Grounded Enemy")
        {
            enemyRigidBody.velocity = new Vector2(-enemySpeed, 0);
        }
    }

    void AirEnemyMovement()
    {
        Vector2 point = currentPosition.position - transform.position;

        //Checks what point it need to go to next and makes it move in said direction
        if (currentPosition == pointB.transform && tag == "Air Enemy")
        {
            enemyRigidBody.velocity = new Vector2(0, enemySpeed);
        }
        else if(currentPosition == pointA.transform && tag == "Air Enemy")
        {
            enemyRigidBody.velocity = new Vector2(0, -enemySpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
    #endregion

    #region Getting Enemy Position
    void EnemyPosition()
    {
        //Actually checks when the enemy reches the potral point
        if(Vector2.Distance(transform.position, currentPosition.position) < 0.5f && currentPosition == pointB.transform)
        {
            currentPosition = pointA.transform;
        }

        if(Vector2.Distance(transform.position, currentPosition.position) < 0.5f && currentPosition == pointA.transform)
        {
            currentPosition = pointB.transform;
        }
    }
    #endregion
}
