using urd;

public class Character : Component
{
	public readonly string characterName;

	// 基础属性
	public float walkSpeed = 1.0f;

	public Character(string characterName) 
		: base("Character")
	{
		this.characterName = characterName;
	}
}
