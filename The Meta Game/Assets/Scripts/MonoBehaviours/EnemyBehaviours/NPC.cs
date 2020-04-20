using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : EnemyBehaviour
{
    public Dialogue firstDialogue, otherDialogue;
    public bool interacted;

    public enum NPCType
    {
        standard,
        restoreHP,
        restoreMP,
        learnSpell,
        learnSkill,
        boostHP,
        boostMP,
        boostStrength,
        boostMagic
    };
    public NPCType npcType;

    public int amt;
    public Spell spell;
    public Skill skill;

    public void Start()
    {
        interacted = false;

        switch(npcType)
        {
            case NPCType.standard:
                animator.SetInteger("npcType", 0);
                break;

            case NPCType.restoreHP:
                animator.SetInteger("npcType", 1);
                break;

            case NPCType.restoreMP:
                animator.SetInteger("npcType", 2);
                break;

            case NPCType.learnSpell:
                animator.SetInteger("npcType", 3);
                break;

            case NPCType.learnSkill:
                animator.SetInteger("npcType", 4);
                break;

            case NPCType.boostHP:
                animator.SetInteger("npcType", 5);
                break;

            case NPCType.boostMP:
                animator.SetInteger("npcType", 6);
                break;

            case NPCType.boostStrength:
                animator.SetInteger("npcType", 7);
                break;

            case NPCType.boostMagic:
                animator.SetInteger("npcType", 8);
                break;
        }
    }

    private void OnEnable()
    {
        animator.SetBool("rpg", true);
    }

    private void OnDisable()
    {
        animator.SetBool("rpg", false);
    }

    public void Interact()
    {
        StartCoroutine(InteractRtn());
    }

    private IEnumerator InteractRtn()
    {
        if (interacted)
        {
            DialogueManager.singleton.StartDialogue(otherDialogue);
        }
        else
        {
            DialogueManager.singleton.StartDialogue(firstDialogue);
        }

        yield return new WaitUntil(() => !DialogueManager.singleton.GetDisplaying());

        bool found = false;

        switch (npcType)
        {
            case NPCType.restoreHP:
                GameController.singleton.Damage(-GameController.singleton.maxHP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.restoreMP:
                GameController.singleton.Cast(-GameController.singleton.maxMP);
                SaveManager.singleton.UpdatePlayerData();
                break;

            case NPCType.boostHP:
                if (!interacted)
                {
                    GameController.singleton.maxHP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMP:
                if (!interacted)
                {
                    GameController.singleton.maxMP += amt;
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostStrength:
                if (!interacted)
                {
                    GameController.singleton.SetStrength(GameController.singleton.GetStrength() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.boostMagic:
                if (!interacted)
                {
                    GameController.singleton.SetMagic(GameController.singleton.GetMagic() + amt);
                    SaveManager.singleton.UpdatePlayerData();
                }
                break;

            case NPCType.learnSpell:
                if (!interacted)
                {
                    for (int i = 0; i < GameController.singleton.spellList.Length && !found; i++)
                    {
                        if (GameController.singleton.spellList[i].spell.spellName.Equals(spell.spellName))
                        {
                            GameController.singleton.spellList[i].unlocked = true;
                            SaveManager.singleton.UpdatePlayerData();
                            found = true;
                        }
                    }
                }
                break;
        }

        if (!interacted)
        {
            interacted = true;
            SaveManager.singleton.UpdateSceneData();
        }
    }
}
