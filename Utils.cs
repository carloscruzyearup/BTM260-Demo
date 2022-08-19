using UnityEngine;

public static class Utils
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 _input) => _isoMatrix.MultiplyPoint3x4(_input);
    
    public static Vector3 CameraFollow(this Vector3 _position, Vector3 target, float smoothSpeed, float delta) => Vector3.Lerp(_position, target, smoothSpeed * delta);
}
