using UnityEngine;
//using VirtualTransform;

namespace VariableValue {
    [System.Serializable]
    public abstract class VariableValue<ValueType> {
        public enum ChangeType { none, addition, selection }
        public enum LimitType { noLimit, turnBack, stop }
        protected delegate ValueType ValueAdd(int index);
        [SerializeField] internal ValueType defaultValue = default;
        [SerializeField] internal ChangeType changeType = ChangeType.addition;
        [SerializeField] internal ValueType addValue = default;
        [SerializeField] internal float magnification = 1f;
        [SerializeField, Range(0, 50)] internal int everyNTimes = 0;
        [SerializeField] internal LimitType limitType = LimitType.noLimit;
        [SerializeField, Range(0, 100)] internal int maxIndex = 0;
        [SerializeField] internal ValueType[] selection = new ValueType[1];
        //private int selectionSize;
        protected ValueAdd valueAdd = delegate (int _index) {
            Debug.LogErrorFormat("Warning/VariableValue/初期化されていません");
            return default;
        };
        public VariableValue(ValueType defaultValue, ValueType addValue, float magnification = 1f, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) {
            this.changeType = ChangeType.addition;
            this.defaultValue = defaultValue;
            this.addValue = addValue;
            this.magnification = magnification;
            this.everyNTimes = everyNTimes;
            this.limitType = limitType;
            this.maxIndex = maxIndex;
        }
        public VariableValue(ValueType defaultValue, ValueType[] selection, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) {
            this.changeType = ChangeType.selection;
            this.defaultValue = defaultValue;
            this.selection = selection;
            this.everyNTimes = everyNTimes;
            this.limitType = limitType;
            this.maxIndex = maxIndex;
        }
        public VariableValue() { }
        public virtual ValueType GetValue(int index) {
            int _index = GetLimitedIndex(index / (everyNTimes + 1));
            switch (changeType) {
                case ChangeType.none:
                    return defaultValue;
                case ChangeType.addition:
                    return valueAdd(_index);
                case ChangeType.selection:
                    if (selection.Length > _index) return selection[_index];
                    else {
                        Debug.LogWarningFormat("Warning/VariableValue/Index out of Selection length");
                        return defaultValue;
                    }
                default:
                    Debug.LogWarningFormat("ChangeTypeError : " + changeType);
                    return defaultValue;
            }
        }
        private int GetLimitedIndex(int index) {
            switch (limitType) {
                default:
                case LimitType.noLimit:
                    return index;
                case LimitType.turnBack:
                    return index % (maxIndex + 1);
                case LimitType.stop:
                    return Mathf.Min(index, maxIndex);
            }
        }
    }
    [System.Serializable]
    public class VariableFloat : VariableValue<float> {
        public VariableFloat(float defaultValue, float addValue, float magnification = 1f, int evetyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, addValue, magnification, evetyNTimes, limitType, maxIndex) { }
        public VariableFloat(float defaultValue, float[] selection, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, selection, everyNTimes, limitType, maxIndex) { }
        public VariableFloat() : base() { }
        public override float GetValue(int index) {
            valueAdd = delegate (int _index) { return defaultValue + magnification * addValue * _index; };
            return base.GetValue(index);
        }
    }
    [System.Serializable]
    public class VariableVector2 : VariableValue<Vector2> {
        public VariableVector2(Vector2 defaultValue, Vector2 addValue, float magnification = 1f, int evetyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, addValue, magnification, evetyNTimes, limitType, maxIndex) { }
        public VariableVector2(Vector2 defaultValue, Vector2[] selection, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, selection, everyNTimes, limitType, maxIndex) { }
        public VariableVector2() : base() { }
        public override Vector2 GetValue(int index) {
            valueAdd = delegate (int _index) { return defaultValue + magnification * addValue * _index; };
            return base.GetValue(index);
        }
    }
    [System.Serializable]
    public class VariableVector3 : VariableValue<Vector3>/*, IVector3Class<int>*/ {
        public VariableVector3(Vector3 defaultValue, Vector3 addValue, float magnification = 1f, int evetyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, addValue, magnification, evetyNTimes, limitType, maxIndex) { }
        public VariableVector3(Vector3 defaultValue, Vector3[] selection, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, selection, everyNTimes, limitType, maxIndex) { }
        public VariableVector3() : base() { }
        public override Vector3 GetValue(int index) {
            valueAdd = delegate (int _index) { return defaultValue + magnification * addValue * _index; };
            return base.GetValue(index);
        }
        public Vector3 GetVector3(int value) => GetValue(value);
    }
    [System.Serializable]
    public class VariableVector4 : VariableValue<Vector4> {
        public VariableVector4(Vector4 defaultValue, Vector4 addValue, float magnification = 1f, int evetyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, addValue, magnification, evetyNTimes, limitType, maxIndex) { }
        public VariableVector4(Vector4 defaultValue, Vector4[] selection, int everyNTimes = 0, LimitType limitType = LimitType.noLimit, int maxIndex = 0) :
            base(defaultValue, selection, everyNTimes, limitType, maxIndex) { }
        public VariableVector4() : base() { }
        public override Vector4 GetValue(int index) {
            valueAdd = delegate (int _index) { return defaultValue + magnification * addValue * _index; };
            return base.GetValue(index);
        }
    }
    [System.Serializable]
    public class VariableQuaternion : VariableValue<Quaternion> {
        public override Quaternion GetValue(int index) {
            valueAdd = delegate (int _index) { return defaultValue * Quaternion.LerpUnclamped(Quaternion.identity, addValue, magnification * _index); };
            return base.GetValue(index);
        }
    }
}
