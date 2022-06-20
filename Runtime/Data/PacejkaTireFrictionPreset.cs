using UnityEngine;
using NaughtyAttributes;

using static Drifter.Utility.DrifterMathUtility;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drifter.Data
{
    [CreateAssetMenu(menuName = "Tools/Drifter/Presets/Tire Friction/Pacejka [Preset]", fileName = "New Pacejka Tire Friction Preset", order = 10)]
    public sealed class PacejkaTireFrictionPreset : BaseTireFrictionPreset
    {
        // GUIDE: Pacejka's formula
        // C = Shape factor
        // D = Peak Factor
        // BCD = Stiffness
        // B = Stiffness factor
        // E = Curvature factor
        // H = Horizontal shift
        // V = Vertical shift
        // Bx1 = composite

        public enum VersionType : byte
        {
            Pac89,
            Pac94,
            Pac2002,
        }

        [Header("References")]
        [SerializeField] TextAsset m_JsonAsset = default;

        [Header("General Settings")]
        [SerializeField] VersionType m_Version = VersionType.Pac94;

        private void OnEnable() => LoadFromJson();

        [ContextMenu("Load From Json")]
        public void LoadFromJson()
        {
            if (m_JsonAsset == null || string.IsNullOrEmpty(m_JsonAsset.text))
                return;

            var data = JObject.Parse(m_JsonAsset.text);

            a0 = data["a0"]?.Value<float?>() ?? 0f;
            a1 = data["a1"]?.Value<float?>() ?? 0f;
            a2 = data["a2"]?.Value<float?>() ?? 0f;
            a3 = data["a3"]?.Value<float?>() ?? 0f;
            a4 = data["a4"]?.Value<float?>() ?? 0f;
            a5 = data["a5"]?.Value<float?>() ?? 0f;
            a6 = data["a6"]?.Value<float?>() ?? 0f;
            a7 = data["a7"]?.Value<float?>() ?? 0f;
            a8 = data["a8"]?.Value<float?>() ?? 0f;
            a9 = data["a9"]?.Value<float?>() ?? 0f;
            a10 = data["a10"]?.Value<float?>() ?? 0f;
            a11 = data["a11"]?.Value<float?>() ?? 0f;
            a12 = data["a12"]?.Value<float?>() ?? 0f;
            a13 = data["a13"]?.Value<float?>() ?? 0f;
            a14 = data["a14"]?.Value<float?>() ?? 0f;
            a15 = data["a15"]?.Value<float?>() ?? 0f;
            a16 = data["a16"]?.Value<float?>() ?? 0f;
            a17 = data["a7"]?.Value<float?>() ?? 0f;

            b0 = data["b0"]?.Value<float?>() ?? 0f;
            b1 = data["b1"]?.Value<float?>() ?? 0f;
            b2 = data["b2"]?.Value<float?>() ?? 0f;
            b3 = data["b3"]?.Value<float?>() ?? 0f;
            b4 = data["b4"]?.Value<float?>() ?? 0f;
            b5 = data["b5"]?.Value<float?>() ?? 0f;
            b6 = data["b6"]?.Value<float?>() ?? 0f;
            b7 = data["b7"]?.Value<float?>() ?? 0f;
            b8 = data["b8"]?.Value<float?>() ?? 0f;
            b9 = data["b9"]?.Value<float?>() ?? 0f;
            b10 = data["b10"]?.Value<float?>() ?? 0f;
            b11 = data["b11"]?.Value<float?>() ?? 0f;
            b12 = data["b12"]?.Value<float?>() ?? 0f;
            b13 = data["b13"]?.Value<float?>() ?? 0f;

            c0 = data["c0"]?.Value<float?>() ?? 0f;
            c1 = data["c1"]?.Value<float?>() ?? 0f;
            c2 = data["c2"]?.Value<float?>() ?? 0f;
            c3 = data["c3"]?.Value<float?>() ?? 0f;
            c4 = data["c4"]?.Value<float?>() ?? 0f;
            c5 = data["c5"]?.Value<float?>() ?? 0f;
            c6 = data["c6"]?.Value<float?>() ?? 0f;
            c7 = data["c7"]?.Value<float?>() ?? 0f;
            c8 = data["c8"]?.Value<float?>() ?? 0f;
            c9 = data["c9"]?.Value<float?>() ?? 0f;
            c10 = data["c10"]?.Value<float?>() ?? 0f;
            c11 = data["c11"]?.Value<float?>() ?? 0f;
            c12 = data["c12"]?.Value<float?>() ?? 0f;
            c13 = data["c13"]?.Value<float?>() ?? 0f;
            c14 = data["c14"]?.Value<float?>() ?? 0f;
            c15 = data["c15"]?.Value<float?>() ?? 0f;
            c16 = data["c16"]?.Value<float?>() ?? 0f;
            c17 = data["c17"]?.Value<float?>() ?? 0f;
            c18 = data["c18"]?.Value<float?>() ?? 0f;
            c19 = data["c19"]?.Value<float?>() ?? 0f;
            c20 = data["c20"]?.Value<float?>() ?? 0f;

            peakSlipRatio = data["peakSlipRatio"]?.Value<float?>() ?? 10f;
            peakSlipAngle = data["peakSlipAngle"]?.Value<float?>() ?? 10f;
        }

        private float a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17;
        private float b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10, b11, b12, b13;
        private float c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15, c16, c17, c18, c19, c20;
        private float peakSlipRatio;
        private float peakSlipAngle;

        public override float CalcLongitudinalForce(float normalForce, float slipRatio) => m_Version switch
        {
            VersionType.Pac94 => CalcLongitudinalForce94(normalForce / 1000f, slipRatio),
            VersionType.Pac89 => CalcLongitudinalForce89(normalForce / 1000f, slipRatio),
            _ => CalcLongitudinalForce94(normalForce / 1000f, slipRatio),
        };

        public override float CalcLateralForce(float normalForce, float slipAngle, float camberAngle = 0) => m_Version switch
        {
            VersionType.Pac94 => CalcLateralForce94(normalForce / 1000f, slipAngle, camberAngle),
            VersionType.Pac89 => CalcLateralForce89(normalForce / 1000f, slipAngle, camberAngle),
            _ => CalcLateralForce94(normalForce / 1000f, slipAngle, camberAngle),
        };

        public override float CalcAligningTorque(float normalForce, float slipAngle, float camberAngle = 0) => m_Version switch
        {
            VersionType.Pac94 => CalcAligningTorque94(normalForce / 1000f, slipAngle, camberAngle),
            VersionType.Pac89 => CalcAligningTorque89(normalForce / 1000f, slipAngle, camberAngle),
            _ => CalcAligningTorque94(normalForce / 1000f, slipAngle, camberAngle),
        };

        private float CalcLongitudinalForce94(float fz, float slipRatio)
        {
            const float DLON = 1f; // Scale Factor
            const float BCDLON = 1f; // Scale Factor

            var fzSqr = Mathf.Pow(fz, 2f);

            var shapeFactor = b0; // [C]
            var peakFactor = (b1 * fzSqr + b2 * fz) * DLON; // [D]
            var stiffness = (b3 * fzSqr + b4 * fz) * Mathf.Exp(-b5 * fz) * BCDLON; // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = b9 * fz + b10; // [H]
            var verticalShift = b11 * fz + b12; // [V]
            var composite = slipRatio + horizontalShift;
            var curvatureFactor = ((b6 * fz + b7) * fz + b8) * (1f - (b13 * Sign(composite))); // [E]

            var result = (peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite))))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        private float CalcLongitudinalForce89(float fz, float slipRatio)
        {
            var fzSqr = Mathf.Pow(fz, 2f);

            var shapeFactor = b0; // [C]
            var peakFactor = b1 * fzSqr + b2 * fz; // [D]
            var stiffness = (b3 * fzSqr + b4 * fz) * Mathf.Exp(-b5 * fz); // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = b9 * fz + b10; // [H]
            var verticalShift = 0f; // [V]
            var composite = slipRatio + horizontalShift; // [Bx1]
            var curvatureFactor = b6 * fzSqr + b7 * fz + b8; // [E]

            var result = peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite)))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        private float CalcLateralForce94(float fz, float slipAngle, float camberAngle)
        {
            const float DLAT = 1f; // Scale Factor
            const float BCDLAT = 1f; // Scale Factor

            var fzSqr = Mathf.Pow(fz, 2f);

            var shapeFactor = a0; // [C]
            var peakFactor = (a1 * fzSqr + a2) * fz * DLAT; // [D]
            var stiffness = a3 * Mathf.Sin(Mathf.Atan(fz / a4) * 2f) * (1f - a5 * Mathf.Abs(camberAngle)) * BCDLAT; // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = a8 * camberAngle + a9 * fz + a10; // [H]
            var verticalShift = a11 * fz * a12 * camberAngle * fz + a13 * fz + a14; // [V]
            var composite = slipAngle + horizontalShift; // [Bx1]
            var curvatureFactor = a6 * fz + a7; // [E]

            var result = peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite)))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        private float CalcLateralForce89(float fz, float slipAngle, float camberAngle)
        {
            var fzSqr = Mathf.Pow(fz, 2f);

            var shapeFactor = a0; // [C]
            var peakFactor = a1 * fzSqr + a2 * fz; // [D]
            var stiffness = a3 * Mathf.Sin(Mathf.Atan(fz / a4) * 2f) * (1f - a5 * Mathf.Abs(camberAngle)); // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = a9 * fz + a10 + a8 * camberAngle; // [H]
            var verticalShift = a11 * fz * camberAngle + a12 * fz + a13; // [V]
            var composite = slipAngle + horizontalShift; // [Bx1]
            var curvatureFactor = a6 * fz + a7; // [E]

            var result = (peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite))))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        private float CalcAligningTorque94(float fz, float slipAngle, float camberAngle)
        {
            const float DVET = 1f; // Scale Factor
            const float BCDVET = 1f; // Scale Factor

            var fzSqr = Mathf.Pow(fz, p: 2f);

            var shapeFactor = c0; // [C]
            //var peakFactor = (c1 * fzSqr + c2 * fz) * (1f - c18 * Mathf.Pow(camberAngle, 2f)); // [D]

            var peakFactor = (c1 * fzSqr + c2 * fz) * DVET; // [D]
            var stiffness = (c3 * fzSqr + c4 * fz) * (1f - c6 * Mathf.Abs(camberAngle)) * 
                Mathf.Exp(-c5 * fz) * BCDVET;  // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = c11 * fz + c12 + c13 * camberAngle; // [H]
            var verticalShift = c14 * fzSqr + c15 * fz + (c16 + fzSqr + c17 * fz) * camberAngle; // [V]
            var composite = slipAngle + horizontalShift; // [Bx1]
            var curvatureFactor = (c7 * fzSqr + (c8 * fz) + c9 - (1f - (c19 * camberAngle) + c20) * 
                Sign(composite)) / (1f - c10 * Mathf.Abs(camberAngle)); // [E]

            var result = peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite)))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        private float CalcAligningTorque89(float fz, float slipAngle, float camberAngle)
        {
            const float DVET = 1f; // Scale Factor
            const float BCDVET = 1f; // Scale Factor

            var fzSqr = Mathf.Pow(fz, p: 2f);

            var shapeFactor = c0; // [C]
            //var peakFactor = (c1 * fzSqr + c2 * fz) * (1f - c18 * Mathf.Pow(camberAngle, 2f)); // [D]

            var peakFactor = (c1 * fzSqr + c2 * fz) * DVET; // [D]
            var stiffness = (c3 * fzSqr + c4 * fz) * (1f - c6 * Mathf.Abs(camberAngle)) *
                Mathf.Exp(-c5 * fz) * BCDVET;  // [BCD]
            var stiffnessFactor = SafeDivision(stiffness, shapeFactor * peakFactor); // [B]
            var horizontalShift = c11 * fz + c12 + c13 * camberAngle; // [H]
            var verticalShift = c14 * fzSqr + c15 * fz + (c16 + fzSqr + c17 * fz) * camberAngle; // [V]
            var composite = slipAngle + horizontalShift; // [Bx1]
            var curvatureFactor = (c7 * fzSqr + (c8 * fz) + c9 - (1f - (c19 * camberAngle) + c20) *
                Sign(composite)) / (1f - c10 * Mathf.Abs(camberAngle)); // [E]

            var result = peakFactor * Mathf.Sin(shapeFactor * Mathf.Atan(stiffnessFactor * composite - curvatureFactor * (stiffnessFactor * composite - Mathf.Atan(stiffnessFactor * composite)))) + verticalShift;

            if (float.IsNaN(result) || float.IsInfinity(result))
                result = 0f;

            return result;
        }

        public override Vector3 GetCombinedForce(float normalForce, float slipRatio, float slipAngle, float camberAngle = 0)
        {
            const float MAX_FORCE = 30000f; // [N]
            const float MAX_TORQUE = 1000f; // [N/m]

            var slipRatioNorm = SafeDivision(slipRatio, peakSlipRatio);
            var slipAngleNorm = SafeDivision(slipAngle, peakSlipAngle);
            var combinedSlip = Mathf.Sqrt(slipRatioNorm * slipRatioNorm + slipAngleNorm * slipAngleNorm);

            if (combinedSlip <= 0f)
                return Vector3.zero;

            var lateralForce = SafeDivision(slipAngleNorm, combinedSlip) * 
                CalcLateralForce(normalForce, combinedSlip * peakSlipAngle, camberAngle);

            var selfAligningTorque = SafeDivision(slipAngleNorm, combinedSlip) * 
                CalcAligningTorque(normalForce, combinedSlip * peakSlipAngle, camberAngle);

            var longitudinalForce = SafeDivision(slipRatioNorm, combinedSlip) * 
                CalcLongitudinalForce(normalForce, combinedSlip * peakSlipRatio);

            return new Vector3
            {
                x = Mathf.Clamp(lateralForce, -MAX_FORCE, MAX_FORCE),
                y = Mathf.Clamp(selfAligningTorque, -MAX_TORQUE, MAX_TORQUE),
                z = Mathf.Clamp(longitudinalForce, -MAX_FORCE, MAX_FORCE),
            };
        }
    }
}