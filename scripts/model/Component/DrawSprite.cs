namespace urd
{
	[RecordObject]
	public class DrawSprite : Component
	{
		[BindComponent]
		private WorldEntity m_worldEntity = null;

		private Sprite _sprite;
		public Sprite sprite { set => _sprite = value; get => _sprite; }

		public DrawSprite(Sprite sprite) : base("DrawSprite") 
		{
			this.sprite = sprite;
		}
	}
}
