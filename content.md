## Abstract
For my project I wanted Unity's ML Agents package in order to train models to drive a simple car through a race course using deep reinforcement learning



## Problem Statement
I'd like to use Unity's ML Agents package in order to train a model that is capable of navigating a randomly generated road effectively. In the process of training this agent, I intend to experiement with how various changes to the training environement and rewards/penalties affect the trained model.



## Related work
Since I was unfamiliar with the ML Agents framework, I ended up going over lots of different walkthough tutorials showing how to use it. Links to videos and articles that I referenced are in README.md

## Methodology
### Part 1: Learning to train models
The first part of this project involved getting familiar with the ML Agents framework. To do this, I went through various tutorials

I followed one in particular. Afterwards, I used many of it's project files as a starting point.
- This tutorial included multiple familiar assets, both the car controller and road tiles

### Part 2: Generating random roads for training
The second part of this project was to experiment with agent and environment settings. I had two main goals:
-I wanted to be able to vary the track during training in order to compare how it affected train time and agent behaviors
-I wanted to be vary hyperparameters and aspects of the agent’s model structure and rewards to draw comparisons

I created a track generator in order to introduce randomness into the training environment
Although there isn’t a dataset in a traditional sense, the track tiles sampled during generation can be changed to produce distinct differences in the environment
To train multiple agents in parallel, the environments are stacked on top of each other

In order to train multiple agents in parallel, I stacked the starting positions of each track on top of each other so that they would not interfere with each other.



## Experiments
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

Notably, I trained most of my models for about 2,000,000 steps (~30 minutes each).

### Specific Tests
Partial Reward between checkpoints (with fixed time)
Although the original code included a reward for reaching each checkpoint, I wanted to add a fraction of the checkpoint reward for moving towards it without reaching it completely
This was implemented by calculating the percentage of the euclidean distance away from the next checkpoint and mapping that value to between the max reward and zero.
Could be adjusted to additionally add a penalty for moving backwards away from a checkpoint that has been reached

I expected that adding this reward to existing rewards would improve performance since the euclidean distance heuristic is consistent given that checkpoints are placed at the border of each tile. To verify this, I trained two models with the same hyperparameters other than the partial reward for checkpoints:

IMAGE

Tileset Subsets
For this test, I wanted to compare how the features of tiles in the tileset could influence how the model trained. I expected that straight tiles would be the easiest to navigate than turns, and that subsets of the full tileset would be easier to learn on.

IMAGE

On a similar note, if more tiles were to be added it would also be interesting to see how the size/length of a tile influenced training since that affects how far apart checkpoints are placed.

Time Bonus for checkpoints 
For this test I wanted to compare how the training process would differ if the time limit was reset on reaching a checkpoint against the time limit being constant.
Note that although both had a small penalty per frame, the fixed time limit made it so that the total penalty from this was constant regardless of progress.

I wasn’t sure which would lead to better results so I wanted to do a direct comparison with and without the time limit bonus. For this test, I used only the small turn tiles as a compromise between the simplicity of only straight tiles and the complexity of using all the tiles.

IMAGE

Track Length
For this test, I varied the length of the track in order to investigate whether there is a significant difference in the training process
Wall Penalty
For this test, I wanted to compare how having a penalty for hitting a wall would affect how the model trained and how the corresponding agent would drive. This was motivated by the observation that before making the car’s steering more responsive, agents trained to control seemed to hit and drive along the wall a lot. I tested having no penalty against having a penalty of -0.1 per second of contact.

IMAGE


Network Hidden Layers
After training a lot of models, I was interested in observing changes in the number and size of hidden layers. The other models were trained with 2 layers with 256 units, so I tried varying the number of layers to be 1, 2, and 4 in one test, and the number of units to be 256, 32, and 4

Notably, the time to train the models didn’t seem to change significantly given the change in network structure despite the number of parameters changing. I think this is because the time used for the physics timesteps bottlenecked the training speed, so I’d be interested in formally investigating whether increasing the time scale or adding more training environments to the training scene would change the training speed or other observable features in the training process.



## Observations
Here are some of my observations:
- Training with all of the pieces and/or with longer tracks slowed down the training when there was a fixed time. Having time be granted after each successive checkpoint significanly improved this though.
- Policy loss graphs for most of my sessions were very noisy, but the agent was still performing well. I'd guess that this is because the value was fairly small from the start, and that the control space was simple enough to learn fairly quickly.\
- Although most of my models were 2 layers with size 256, it seemed that reducing layers to 1 actually increased performance. i'd figure this is becasuse input and output sizes are small in comparison.



## Demo *(maybe)*



## Extensions
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
