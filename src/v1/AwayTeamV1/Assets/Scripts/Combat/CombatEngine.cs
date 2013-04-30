public abstract class CombatEngine
{
    public abstract CombatResult ResolveCombat(
        Actor attacker,
        Actor defender,
        Map map);
}

