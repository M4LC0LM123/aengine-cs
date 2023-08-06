package com.aengineeditor.game;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.*;
import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g3d.*;
import com.badlogic.gdx.graphics.g3d.attributes.ColorAttribute;
import com.badlogic.gdx.graphics.g3d.environment.DirectionalLight;
import com.badlogic.gdx.graphics.g3d.utils.CameraInputController;
import com.badlogic.gdx.graphics.g3d.utils.ModelBuilder;
import com.badlogic.gdx.math.Vector3;
import com.badlogic.gdx.math.collision.Ray;
import com.badlogic.gdx.utils.ScreenUtils;

import javax.swing.*;
import java.awt.*;

public class aengineeditor extends ApplicationAdapter {
	public PerspectiveCamera camera;
	public CameraInputController camController;
	public ModelBatch batch;
	public SpriteBatch spriteBatch;
	public Environment environment;

	public Ray mouseRay;
	public BitmapFont font;

	public String helpString;
	
	@Override
	public void create () {
		camera = new PerspectiveCamera(90, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
		camera.position.set(10f, 10f, 10f);
		camera.lookAt(0,0,0);
		camera.near = 1f;
		camera.far = 300f;
		camera.update();
		camController = new CameraInputController(camera);
		Gdx.input.setInputProcessor(camController);

		batch = new ModelBatch();
		spriteBatch = new SpriteBatch();

		environment = new Environment();
		environment.set(new ColorAttribute(ColorAttribute.AmbientLight, 0.4f, 0.4f, 0.4f, 1f));
		environment.add(new DirectionalLight().set(0.8f, 0.8f, 0.8f, -1f, -0.8f, -0.2f));

		mouseRay = new Ray();
		mouseRay = camera.getPickRay(Gdx.input.getX(), Gdx.input.getY());

		font = new BitmapFont();

		helpString = "Controls:\nAdd object: Space\nRemove object: Backspace\nMove/Scale: Up, Down, Left, Right, PgUp, PgDn\nSave: Ctrl + S\nLoad: Ctrl + L";
	}

	@Override
	public void render () {
		camController.update();

		mouseRay = camera.getPickRay(Gdx.input.getX(), Gdx.input.getY());

		World.update(mouseRay);

		if (Gdx.input.isKeyJustPressed(Input.Keys.SPACE)) {
			World.addObject(Config.currentId, Color.GREEN);
		}
		if (Gdx.input.isKeyJustPressed(Input.Keys.BACKSPACE) && Config.selectedObject != null) {
			World.removeObject(Config.selectedObject);
		}

		if (Gdx.input.isKeyJustPressed(Input.Keys.F1)){
			Config.currentMode = Mode.ROAM;
		}
		if (Gdx.input.isKeyJustPressed(Input.Keys.F2)){
			Config.currentMode = Mode.MOVE;
		}
		if (Gdx.input.isKeyJustPressed(Input.Keys.F3)){
			Config.currentMode = Mode.SCALE;
		}

		if (Gdx.input.isKeyPressed(Input.Keys.CONTROL_LEFT) && Gdx.input.isKeyJustPressed(Input.Keys.S)) {
			FileDialog fd = new FileDialog(new JFrame(), "Choose a file", FileDialog.SAVE);
			fd.setFile("*.json");
			fd.setVisible(true);
			String filename = fd.getFile();
			if (filename == null) {
				System.out.println("You cancelled the choice");
			} else {
				System.out.println("saved");
				World.saveObjectsToJson(fd.getDirectory() + filename);
			}
		}

		if (Gdx.input.isKeyPressed(Input.Keys.CONTROL_LEFT) && Gdx.input.isKeyJustPressed(Input.Keys.L)) {
			FileDialog fd = new FileDialog(new JFrame(), "Choose a file", FileDialog.LOAD);
			fd.setFile("*.json");
			fd.setVisible(true);
			String filename = fd.getFile();
			if (filename == null) {
				System.out.println("You cancelled the choice");
			} else {
				System.out.println("loaded");
				World.loadObjectsFromJson(fd.getDirectory() + filename);
			}
		}

		Config.changeId();

		Gdx.gl.glViewport(0, 0, Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
		Gdx.gl.glClear(GL30.GL_COLOR_BUFFER_BIT | GL30.GL_DEPTH_BUFFER_BIT);
		Gdx.gl.glEnable(GL30.GL_BLEND);
		Gdx.gl.glBlendFunc(GL30.GL_SRC_ALPHA, GL30.GL_ONE_MINUS_SRC_ALPHA);

		batch.begin(camera);
		World.render(batch, environment);

		Rendering.drawGrid(100, 100, 1, 1, Color.WHITE, batch, environment);
		Rendering.drawDebugAxies(5, batch, environment);

		Rendering.drawLine(mouseRay.origin, mouseRay.direction, new Color(0, 0, 1, 0.1f), batch, environment);
		batch.end();

		spriteBatch.begin();
		font.draw(spriteBatch, "fps: " + Gdx.graphics.getFramesPerSecond(), 10, Gdx.graphics.getHeight() - font.getScaleY() - 10);
		font.draw(spriteBatch, "mode: " + Config.currentMode, 10, Gdx.graphics.getHeight() - font.getScaleY() - 30);
		font.draw(spriteBatch, "id: " + Config.currentId, 10, Gdx.graphics.getHeight() - font.getScaleY() - 50);
		if (Config.selectedObject != null) font.draw(spriteBatch, "object id: " + Config.activeId, 10, Gdx.graphics.getHeight() - font.getScaleY() - 70);
		if (Config.selectedObject != null) font.draw(spriteBatch, "object position: " + Config.selectedObject.position, 10, Gdx.graphics.getHeight() - font.getScaleY() - 90);
		if (Config.selectedObject != null) font.draw(spriteBatch, "object scale: " + Config.selectedObject.scale, 10, Gdx.graphics.getHeight() - font.getScaleY() - 110);
		font.draw(spriteBatch, helpString, 10, Gdx.graphics.getHeight() - font.getScaleY() - 200);
		spriteBatch.end();
	}
	
	@Override
	public void dispose () {
		World.dispose();
	}
}
