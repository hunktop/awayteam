using UnityEngine;
using System.Collections;

public class PongDemo : MonoBehaviour {
	
	public PDGame game;
	
	// Use this for initialization
	void Start () {
		FutileParams fparams = new FutileParams(true, true, true, true);
		fparams.AddResolutionLevel(480.0f, 1.0f, 1.0f, "");
		fparams.origin = new Vector2(0.5f, 0.5f);
		Futile.instance.Init(fparams);
		
		Futile.atlasManager.LoadAtlas("Atlases/PongDemo");
		
		game = new PDGame();
	}
	
	// Update is called once per frame
	void Update () {
		game.Update(Time.deltaTime);
	}
}

public class PDPaddle : FSprite {
	public string name;
	public int score;
	public float defaultVelocity;
	public float currentVelocity;
	public string upKey;
	public string downKey;
	
	/* this is a note purely for me.... Ian.
	 * for the following, since FSprite requires an argument when being
	 * constructed, we use the '... : base()' bit to pass the appropriate
	 * argument so that all the parent classes are happy. */ 
	public PDPaddle(string name, string upKey, string downKey) : base("paddle.png") {
		this.name = name;
		
		defaultVelocity = Futile.screen.height; // so... paddle moves 1 screen height per second
		currentVelocity = defaultVelocity;
		this.upKey = upKey;
		this.downKey = downKey;
	}
	
	public void Update(float dt) {
		if (Input.GetKey(this.upKey)) { this.y += dt*this.currentVelocity; }
		if (Input.GetKey(this.downKey)) { this.y -= dt*this.currentVelocity; }
	}
}

public class PDBall : FSprite {
	public float xVelocity;
	public float yVelocity;
	public float defaultVelocity;
	public float currentVelocity;
	
	public PDBall() : base("ball.png") {
		defaultVelocity = 100.0f;
		currentVelocity = defaultVelocity;
	}
}

public class PDGame {
	public PDPaddle player1;
	public PDPaddle player2;
	public PDBall ball;
	
	public PDGame() {
		player1 = new PDPaddle("player1", "w", "s");
		player2 = new PDPaddle("player2", "up", "down");
		ResetPaddles();
		
		ball = new PDBall();
		ResetBall();
		
		Futile.stage.AddChild(player1);
		Futile.stage.AddChild(player2);
		Futile.stage.AddChild(ball);
	}
	
	public void Update(float dt) {
		ball.x += dt*ball.xVelocity;
		ball.y += dt*ball.yVelocity;
		
		player1.Update(dt);
		player2.Update(dt);
	}
	
	public void ResetPaddles() {
		player1.x = -Futile.screen.halfWidth + player1.width/2;
		player1.y = 0;
		player2.x = Futile.screen.halfWidth - player2.width/2;
		player2.y = 0;
	}
	
	public void ResetBall() {
		ball.x = 0;
		ball.y = 0;
		// grabbed the following from the tutorial. it gives the ball a default velocity in a random direction, within 45 degrees on either side. there's likely a better way to do it.
		ball.yVelocity = (ball.defaultVelocity/2) - (RXRandom.Float() * ball.defaultVelocity);
		ball.xVelocity = Mathf.Sqrt((ball.defaultVelocity*ball.defaultVelocity) - (ball.yVelocity*ball.yVelocity)) * (RXRandom.Int(2) * 2 - 1);
	}
}