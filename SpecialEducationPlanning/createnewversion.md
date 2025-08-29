# How to Create a New Version of EducationView
The versioning scheme used in EducationView is SemVer - https://semver.org/

In order to create a new version of EducationView, please follow the steps below. The example given is a patch version change from 1.0.0 to 1.0.1. Of course, this is just an example and if a different version change is required please make the correct SemVer version change. 

1. When the develop branch contains the code that is ready to be released as version 1.0.1, create a new branch from develop called 'releases/1_0_1'.
2. In the releases/1_0_1 branch, increment the version number from 1.0.0 to 1.0.1. This needs to happen in the following files:
    - SpecialEducationPlanning
/version.props
    - SpecialEducationPlanning
.Web/package.json
    - SpecialEducationPlanning
.Web/src/assets/configuration.js

3. Raise a Pull Request to merge releases/1_0_1 into main. Do not delete the releases/1_0_1 branch. 
4. Once releases/1_0_1 has been merged into main, add a git tag to the main branch. The value of the tag should be the new version - '1.0.1'. 
5. Make sure the tag is pushed to origin. 
6. Finally, merge releases/1_0_1 into develop. 
