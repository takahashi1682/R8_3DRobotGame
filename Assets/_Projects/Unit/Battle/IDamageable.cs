using VContainer;

namespace _Projects.Unit.Battle
{
    public interface IDamageable
    {
        public IObjectResolver Owner { get; } // ダメージを受ける本人
        public bool TakeDamage(IDamageSource source);
    }
}