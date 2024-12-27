#!/bin/bash

gource \
	-s 1 \
	-2000x1124 \
	--stop-at-end \
	--key \
	--highlight-users \
	--hide mouse \
	--background-colour 000000 \
	--output-ppm-stream - \
	--file-filter '.*\.(meta|asset)$' \
	--background-image 'DarkerCover.png' \
	--output-framerate 30 |
	ffmpeg -y -r 30 -f image2pipe -vcodec ppm -i - -vcodec libx264 -pix_fmt yuv420p -crf 1 -threads 0 -bf 0 gource_record.mp4
