import cv2

print("Image to cubemap generator")

run = True
while run:
    new = input("new? y/n: ")

    if (new == "y"):
        panorama_img = cv2.imread(input("Image path: "))

        # Calculate width and height of each face (assuming the panorama is equirectangular)
        height, width = panorama_img.shape[:2]
        face_width = width // 4  # Assuming a 2:1 panorama aspect ratio
        face_height = height // 3

        # Create empty images for cubemap faces
        faces = {
            "front":   panorama_img[face_height:2*face_height, face_width:2*face_width],
            "back":    panorama_img[face_height:2*face_height, 3*face_width:],
            "left":    cv2.rotate(panorama_img[face_height:2*face_height, :face_width], cv2.ROTATE_90_CLOCKWISE),
            "right":   cv2.rotate(panorama_img[face_height:2*face_height, 2*face_width:3*face_width], cv2.ROTATE_90_COUNTERCLOCKWISE),
            "top":     cv2.rotate(panorama_img[:face_height, face_width:3*face_width], cv2.ROTATE_90_CLOCKWISE),
            "bottom":  cv2.rotate(panorama_img[2*face_height:, face_width:3*face_width], cv2.ROTATE_90_COUNTERCLOCKWISE)
        }

        print("Created cubemap")
        ext = input("File format type: ")

        # Save cubemap faces
        for face_name, face_img in faces.items():
            cv2.imwrite(f"{face_name}.{ext}", face_img)

        print("Exported cubemap")
    else:
        exitB = input("exit? y/n: ")
        if (exitB == "y"): run = False
