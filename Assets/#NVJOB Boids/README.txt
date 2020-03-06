#NVJOB Simple Boids is a simulation of the behavior of flocks of birds and fish.
This technology is not real Boids, it's pseudo Boids (not mathematical model), but the basic Boids rules (separation, alignment, cohesion) are followed.

Animation of birds, fish and butterflies implemented using shaders. The asset includes two shaders, one for birds and butterflies, the second for fish.

Features:
- One script for all flocking.
- Good performance.
- A large number of flocking objects.
- Random behavior.
- Customization for different types of flocks.
- Reaction of flocks to danger.
- Animation implemented using a shader.

I recommend importing this asset into a new project. Study the operation of the asset and copy the components you need to your project.

There are five examples in the asset, for understanding the operation and settings.
Asset already includes several models (three fish models, one bird model and one butterfly model). In fact, you can use any model, see the example "Fish Boids Danger", the shark is animated using a shader.

#NVJOB Simple Boids allows you to create many flocks and many objects in flocks, and it is all controlled by one script. I got good performance by calculating all the flocks in one script, but nevertheless, if you plan to use tens of thousands of birds, this will certainly affect the performance decrease.

Full instructions - https://nvjob.github.io/unity/nvjob-boids

Distributed with MIT License.