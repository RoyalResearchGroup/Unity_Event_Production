## Install unity 2023.2.15f1
## Install Anaconda
## Create conda environment and install necessary modules:
- create a new environment with necessary python and instantly activate it: 
```sh
conda create -n mlagents python=3.10.12 && conda activate mlagents
```

- install torch within the environment
```sh
pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
```
- cd to the path setupResources
- install the two packages within the environment
 ```sh
python -m pip install ./ml-agents-envs
python -m pip install ./ml-agents
```