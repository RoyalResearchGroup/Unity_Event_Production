##  Install Unity (2023.2 or later)
- [Download](https://unity3d.com/get-unity/download) and install Unity.

## Install Anaconda 
1. [Download](https://www.anaconda.com/download) and install anaconda

## Import the the environment in Anaconda
1. Save the ml-agents-environment.yml
[ml-agents-environment.yml](./_resources/ml-agents-environment.yml)
1. Navigate to the directory where your environment YAML file is located.
2. Open the file and adapt the path in the last line under `prefix` (find the location of your environments using `conda info`)
3. Create the environment
```sh
conda env create -f ml-agents-environment.yml
```
4. Activate the imported environment
```sh
conda activate ml-agents-environment
```
5. Verify installation: You can verify that the environment has been imported successfully by listing the installed packages using the conda list command 
```sh
conda list
```
or by checking the Anaconda Navigator GUI.

## Clone this repository  
```sh
git clone --branch release_21 https://github.com/Unity-Technologies/ml-agents.git
```

To install the `mlagents` Python package, activate your virtual environment and
run from the command line:

```sh
cd /path/to/ml-agents
python -m pip install ./ml-agents-envs
python -m pip install ./ml-agents
```

## Install the `com.unity.ml-agents` Unity package
You can add the `com.unity.ml-agents` package (from the repository that you just cloned) to your
project by:

1. navigating to the menu `Window` -> `Package Manager`.
1. In the package manager window click on the `+` button on the top left of the packages list).
1. Select `Add package from disk...`
1. Navigate into the `com.unity.ml-agents` folder.
1. Select the `package.json` file.

## Install the `com.unity.ml-agents.extensions` Unity package
To install the `com.unity.ml-agents.extensions` package, you need to first
clone the repo and then complete a local installation similar to what was
outlined in the previoussection.



Now you are able to run the examples form this [guide](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Getting-Started.md)



# Troubleshooting

## Unable to create the environment
If your machine does **not** have a nVidia GPU installed: Change this line `- torch==2.2.2+cu121` to `- torch==2.2.2`

## Unable to install local package `ml-agents-envs`
Try creating a new environment variable `SETUPTOOLS_USE_DISTUTILS` and setting it to `stdlib`.