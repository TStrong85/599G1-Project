## Details
### Part 1
The first part of this project involved getting familiar with the ML Agents framework. To do this, I went through various tutorials

I followed one in particular. Afterwards, I used many of it's project files as a starting point.
- This tutorial included multiple familiar assets, both the car controller and road tiles


### Part 2
The second part of this project was to experiment with agent and environment settings. I had two main goals:
-I wanted to be able to vary the track during training in order to compare how it affected train time and agent behaviors
-I wanted to be vary hyperparameters and aspects of the agent’s model structure and rewards to draw comparisons

I created a track generator in order to introduce randomness into the training environment
Although there isn’t a dataset in a traditional sense, the track tiles sampled during generation can be changed to produce distinct differences in the environment
To train multiple agents in parallel, the environments are stacked on top of each other

After creating a generator, I varied aspects of the generation to observe how agents trained:

- Tilesets (What track pieces are sampled to randomize the tracks)
  - only straight pieces
  - only small curved pieces
  - only curved pieces
  - all tiles
- Track Length (How many tiles are in the track)
- Timer + bonus time for reaching a checkpoint (How much time does the agent have to complete the track)
- How often the track is regenerated (measured in number of episodes completed using it)

Additionally, I varied aspects of the reward functions
- Penalty each step (to encourage higher speed)
- Penalty when hitting a wall (to encourage obstacle avoidance)
- Reward for reaching checkpoints
- Reward for completing the track



## Conclusion
Here are some of my observations:
- Training with all of the pieces and/or with longer tracks slowed down the training when there was a fixed time. Having time be granted after each successive checkpoint significanly improved this though.
- Policy loss graphs for most of my sessions were very noisy, but the agent was still performing well. I'd guess that this is because the value was fairly small from the start, and that the control space was simple enough to learn fairly quickly.\
- Although most of my models were 2 layers with size 256, it seemed that reducing layers to 1 actually increased performance. i'd figure this is becasuse input and output sizes are small in comparison.

If I were to continue this project, I’d be interested in a few areas:
- Experiment more with types of observations passed to the model
  - Change the number and position of raycasts
  - Give more pose data about checkpoints and physics data about self
  - Try image observations collected from a camera on the front bumper
- Creating and incorporating more track tiles in order to create more varied tracks and challenging obstacles
  - For example: varying the width of the road, adding changes in elevation, or creating hazards that reduce speed or end the training episode
- Making competitive agents that can collide with one another when driving on the same track
  - Maybe more complex behaviors like passing / blocking could arise, especially if some sort of drafting mechanic is included
- Attempting to tune handling for the car based on the performance of agents as they train with it
  - Can adjust acceleration, top speed, and steering responsiveness and compare on different types of tracks/tilesets
  - Possibly create and train another model to adjust these parameters, either to help or hinder the agent driving
- Implement a variety of car controllers to train models with. The controller used here is very arcady in it’s calculations, but more complex calculations can also be introduced to change how the controller handles
  - Per-wheel suspension and weight shifting
  - Lateral wheel friction and slip curves
  - Reduced steering response at high speed
