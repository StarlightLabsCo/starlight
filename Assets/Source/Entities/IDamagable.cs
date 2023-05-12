public interface IDamagable : IHealth
{
    void TakeDamage(int damage);
    void Die();
}