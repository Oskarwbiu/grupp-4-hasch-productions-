using UnityEngine;
using UnityEngine.Rendering;

public class OilScript : MonoBehaviour
{
    private Vector2 target;
    private float moveSpeed;

    private Vector3 curveStartPoint;

    private AnimationCurve curve;
    private AnimationCurve axisCorrectionCurve;
    private AnimationCurve projSpeed;
    private float curveMaxHeight;

    private Vector3 projectileMoveDir;
    private float maxMoveSpeed;

    private void Start()
    {
        curveStartPoint = transform.position;
    }
    void Update()
    {
        UpdatePosition();
        UpdateRotation();

        if (Vector2.Distance(transform.position, target) < 0.3f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateRotation()
    {
        float angle = Mathf.Atan2(projectileMoveDir.y, projectileMoveDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void UpdatePosition()
    {
        Vector3 curveRange = (Vector3)target - curveStartPoint;
        if (curveRange.x < 0)
        {
            moveSpeed *= -1;
        }

        float nextPositionX = transform.position.x + moveSpeed * Time.deltaTime;
        float nextPositionXNormalized = (nextPositionX - curveStartPoint.x) / curveRange.x;

        float nextPositionYNormalized = curve.Evaluate(nextPositionXNormalized);

        float nextPositionYCorrectionNormalized = axisCorrectionCurve.Evaluate(nextPositionXNormalized);
        float nextPositionYCorrectionAbsolute = nextPositionYCorrectionNormalized * curveRange.y;

        float nextPositionY = curveStartPoint.y + nextPositionYNormalized * curveMaxHeight;

        CalculateSpeed(nextPositionXNormalized);

        Vector3 newPos = new Vector3(nextPositionX, nextPositionY, 0);

        projectileMoveDir = newPos - transform.position;

        transform.position = newPos;
    }

    private void CalculateSpeed(float nextPositionXNormalized)
    {
        float nextMoveSpeedNormalized = projSpeed.Evaluate(nextPositionXNormalized);

        moveSpeed = nextMoveSpeedNormalized * maxMoveSpeed;
    }

    public void InitializeProjectile(Vector2 pos, float speed, AnimationCurve trajectory, AnimationCurve axisCorrection, AnimationCurve projectileSpeed, float trajectoryMaxHeight)
    {
        target = pos;
        maxMoveSpeed = speed;
        curve = trajectory;
        axisCorrectionCurve = axisCorrection;
        projSpeed = projectileSpeed;

        float xDistanceToTarget = target.x - transform.position.x;
        curveMaxHeight = Mathf.Abs(xDistanceToTarget) * trajectoryMaxHeight;
    }
}
