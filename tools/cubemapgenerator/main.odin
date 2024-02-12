package main

import "core:fmt"
import "core:strings"
import "vendor:stb"
import "core:os"
import stbi "vendor:stb/image"

u_charptr :: [^]byte
u_char :: [256]byte
str_empty :: " "

readln :: proc() -> string {
	buf: ^u_char = new(u_char)
	n, err := os.read(os.stdin, buf[:])

	if (err < 0) {
		fmt.println("Failed to read input")
		return str_empty
	}

	return strings.trim(string(buf[:n]), "\n\t\r")
}

main :: proc() {
	fmt.println("Image to cubemap generator")

	run := true
	for run {
		fmt.printf("new? y/n: ")
		new_cubemap := readln()

		if (new_cubemap == "y") {
			width, height, channels: i32
			fmt.printf("Enter image path: ")
			path := strings.clone_to_cstring(readln())
			img: u_charptr = stbi.load(path, &width, &height, &channels, 3)

			if (img == nil) {
				fmt.println("Error loading image")
				fmt.println("stbi error: ", stbi.failure_reason())
				return
			}

			if (f32(width) / f32(height) < 2.0) {
				fmt.println("Image aspect ratio must be atleast 2:1")
				stbi.image_free(img)
				return
			}

			faceWidth: i32 = min(width / 4, height / 3)
			faceHeight: i32 = faceWidth

			fmt.println("Preparing...")

			fmt.printf("Set cubemap name: ")
			name := readln()

			fmt.printf("Choose file type: ")
			fileType := readln()

			faceNames := [?]string{"front", "back", "left", "right", "top", "bottom"}
			for i: i32 = 0; i < 6; i += 1 {
				face: u_charptr = make(u_charptr, faceWidth * faceHeight * channels)

				startX: i32 = (i % 4) * faceWidth
				startY: i32 = (i / 4) * faceHeight
				for y: i32 = 0; y < faceHeight; y += 1 {
                    for x: i32 = 0; x < faceWidth; x += 1 {
                        srcX := startX + x
                        srcY := startY + y
                        dstIndex := (y*faceWidth + x) * channels
                        srcIndex := (srcY*width + srcX) * channels

                        if srcX < 0 || srcX >= width || srcY < 0 || srcY >= height {
                            fmt.println("Source index out of bounds")
                            continue
                        }

                        for c: i32 = 0; c < channels; c += 1 {
                            face[dstIndex+c] = img[srcIndex+c]
                        }
                    }
                }

				fmt.println("Writing...")

				os.make_directory(name)
				dir: string = strings.concatenate([]string{name, "/", name, "_", faceNames[i], ".", fileType})
				
				if (fileType == "jpg") {
					stbi.write_jpg(strings.clone_to_cstring(dir), faceWidth, faceHeight, channels, face, 100)
				} else if (fileType == "png") {
					stbi.write_png(strings.clone_to_cstring(dir), faceWidth, faceHeight, channels, face, faceWidth * channels)
				} else if (fileType == "bmp") {
					stbi.write_bmp(strings.clone_to_cstring(dir), faceWidth, faceHeight, channels, face)
				}
			}

			stbi.image_free(img)
			fmt.println("Generated cubemaps")
		} else {
			fmt.printf("exit? y/n: ")
			if (readln() == "y") { run = false }
		}
	}
}