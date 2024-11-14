all:	
	echo "cd N-puzzle && dotnet run" > npuzzle
	chmod +x npuzzle

clean:
	dotnet clean
	rm npuzzle

fclean: clean

re: clean all

.PHONY: all clean fclean re
