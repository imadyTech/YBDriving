# YBDriving
Vision-based Autonomous Driving simulation among Unity virtual environment with Reinforcement Learning

Python Scripts:
In the YBAutoDriving\Assets\Scripts\Python folder.


---------------------User Manual--------------------------
1.	Hardware Requirements  
The “YB-Driving” can be executed with the following configuration:  
------------------------------------------------!  
Part	          Recommended Configuration       |  
------------------------------------------------|  
CPU	            12700H or above	10800H          |  
Memory	        32GB or above                   |  
Graphics Card	  Nvidia 3070Ti or above          |  
Hard Disk	      NVME PCIE gen.4	NVME PCIE gen.3 |  
Network  	      Not Required                    |  
OS	            Windows 11	                    |  
------------------------------------------------|  
  
3.	Software Requirements  
Unity Engine  	Unity 2021 or newer (you may try Unity 2019 but not guranteed)  
                The plug-in SDK “TMPro” is required.  
Python: 	      Python 3.9 – 3.11  
Python IDE:    	VsCode  
Yolo	          Yolov8 is recommended, but lower version is ok  
  
Environment Assets:	Included in project solution files  


4.	Installation and execution  
Step1. Download the project files:  
 ![image](https://github.com/imadyTech/YBDriving/assets/7894361/4903b643-f958-4149-8296-c389ffb6f880)  
  
Step2. Use installed Unity Engine open the folder “YBAutoDriving” which contains Unity project.  
Step3. Assign the output path of the training images:  
 ![image](https://github.com/imadyTech/YBDriving/assets/7894361/07065578-2321-4d76-a6f1-a289c2084827)  

Step4. Run the Unity project, and randomly click on the top-down map to set destination for the car. The qualified vision will automatically labelled and output to the set training image path.  
Step5. Use your python IDE to open the “yolov8” folder, and run the “ybdriving.ipynb” script. Keep in mind to update the training image path. The yolo model will be trained.  
Step6. Check the “runs” folder of the python project, and find the training result.  
Step7. As an option, you may use yolo to test some image with traffic signs.  

  
Copyright:  
The project is opensource available at https://github.com/imadyTech/YBDriving under MIT license.  
The copyright belong to the respective authors.  
  
  
## Credit:  
Unity asset: SimplePoly City - Low Poly Assets:  
https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899  
Provided by VenCreations  
www.vencreations.com/  
contact@vencreations.com  
https://assetstore.unity.com/publishers/19573  
Under Unity Assetstore Standard EULA  
https://unity.com/cn/legal/as-terms  
  
2021 BMW M4 Competition:  
Ricy  
https://sketchfab.com/ngon_3d  
https://sketchfab.com/3d-models/2021-bmw-m4-competition-d3f07b471d9f4a2c9a2acf79d88a3645  
  
Traffic Signs:  
Produced by Zhenqun, Shen with tripolygon_uModeler  
imadytech@gmail.com  
Offered under MIT License  
