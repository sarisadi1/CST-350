import os

# Replace 'your_folder_path' with the path to your folder
folder_path = "D:/C#/app/wwwroot/images"

# List all files in the folder
file_names = [f for f in os.listdir(folder_path) if os.path.isfile(os.path.join(folder_path, f))]

# Print the file names
for file_name in file_names:
    print(file_name)
