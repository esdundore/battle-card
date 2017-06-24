public class DamageCommand : Command
{
    private MonsterManager targetMonster;
    private int damage;


    public DamageCommand(MonsterManager targetMonster, int damage)
    {
        this.targetMonster = targetMonster;
        this.damage = damage;
    }

    public override void StartCommandExecution()
    {
        DamageEffect.CreateDamageEffect(targetMonster.transform.position, damage);
        Command.CommandExecutionComplete();
    }
}
