import numpy as np
from copy import deepcopy

def calcNextIndex (row, col, n):
    if col < n-1:
        col += 1
    else:
        row += 1
        col = row
    
    return (row, col)

def checkCol (matrix, row, col):
    for i in range(row):
        if matrix[i][col] == matrix[row][col]:
            return False

def checkRow (matrix, row, col):
    for i in range(col):
        if matrix[row][i] == matrix[row][col]:
            return False

def checkTranspose (matrix, n):
    for row in range(n):
        for col in range(n):
            if not matrix[row][col] == matrix[col][row]:
                return False

def checkValidMatrix (matrix, n):
    for col in range(n):
        if(not matrix[0][col] == col):
            return False
    
    for row in range(n):
        if(not matrix[row][0] == row):
            return False
    
    # remove comments if looking for Klein tables:
    # for i in range(n): 
    #     if(not matrix[i][i] == 0):
    #         return False
    
    # not necessary with new algorithm:
    # if checkTranspose(matrix, n) == False:
    #     return False
    
    return True

def fill_matrix (matrix_list, matrix, row, col, n):
    if(col >= n or row >= n):
        if checkValidMatrix(matrix, n) == True:
            # matrix_list.append('1')
            matrix_list.append(deepcopy(matrix))
            print (matrix)
            print ()
        return
    
    for i in range(n):
        matrix[row][col] = i
        matrix[col][row] = i

        if(checkCol(matrix, row, col) == False or checkRow(matrix, row, col) == False):
            continue

        (new_row, new_col) = calcNextIndex(row, col, n)
        fill_matrix(matrix_list, matrix, new_row, new_col, n)
    
    return

# Initialization of parameters:
n = 8
matrix_list = []
row = 1
col = 1

# Initialization of matrix:
matrix = np.random.randint(1, size=(n,n))
for i in range(n):
    matrix[0][i] = i
    matrix[i][0] = i

# Main:
fill_matrix(matrix_list, matrix, row, col, n)
print (len(matrix_list), "matrices found. Some are similar, but some are different, so the number is actually lower.")
