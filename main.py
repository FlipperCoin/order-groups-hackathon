from sympy.utilities.iterables import multiset_permutations
import numpy as np

def isAbel(matrix):
    for i in range(2,len(matrix)):
        for j in range(1,i):
            if matrix[i][j] != matrix[j][i]:
                return False

    return True

def filterAbel(matrix):
    if (matrix == np.transpose(matrix)).all():
        return True

def isAss(mat,n):
    for i in range(0,n):
        for j in range(0,n):
            for k in range(0,n):
                if (mat[mat[i][j]][k] != mat[i][mat[j][k]]):
                    return False

    return True

def nani():
    m = [[0, 1, 2, 3, 4],
        [1, 0, 3, 4, 2],
        [2, 3, 4, 1, 0],
        [3, 4, 0, 2, 1],
        [4, 2, 1, 0, 3]]
    isAss(m,len(m))

def evalval(val,val1,val2):
    return val2 if val is val1 else (val1 if val is val2 else val)

def rowswitch(m,i1,i2):
    for j in range(0,len(m)):
        tmp = m[i1][j]
        m[i1][j] = m[i2][j]
        m[i2][j] = tmp

def colswitch(m,j1,j2):
    for i in range(0,len(m)):
        tmp = m[i][j1]
        m[i][j1] = m[i][j2]
        m[i][j2] = tmp

def swapvars(m,j1,j2):
    n = len(m)

    for i in range(0,n):
        for j in range(0,n):
            m[i][j] = evalval(m[i][j],j1,j2)

    for i in range(0,n):
        for j in range(i+1, n):
            if m[i][0] > m[j][0]:
                rowswitch(m,i,j)
    
    for i in range(0,n):
        for j in range(i,n):
            if m[0][i] > m[0][j]:
                colswitch(m,i,j)

def matrixEquals(m1, m2):
    n = len(m1)
    for i in range(0,n):
        for j in range(0,n):
            if m1[i][j] != m2[i][j]:
                return False
    
    return True

def getPermutations(n):
    permutations = []
    for permutation in multiset_permutations(np.array(range(0,n))):
        permutation_dict = {}
        for i in range(0,n):
            permutation_dict[i]=permutation[i]
        permutations.append(permutation_dict)
    return permutations

def permutateMatrix(m, permutation):
    n = len(m)
    for i in range(0,n):
        for j in range(0,n):
            m[i][j] = permutation[m[i][j]]
    
    for i in range(0,n):
        for j in range(i+1, n):
            if m[i][0] > m[j][0]:
                rowswitch(m,i,j)
    
    for i in range(0,n):
        for j in range(i,n):
            if m[0][i] > m[0][j]:
                colswitch(m,i,j)

def compareMatrices(m1, m2):
    n1 = len(m1)
    n2 = len(m2)
    if n1 != n2:
        return False
    
    if isAbel(m1) ^ isAbel(m2):
        return False
    
    n = n1
    permutations = getPermutations(n)
    count = 0
    m1_permutation = m1
    while not matrixEquals(m1_permutation,m2):
        if count == len(permutations):
            return False
        
        m1_permutation = np.array(m1,copy=True).tolist()
        permutateMatrix(m1_permutation, permutations[count])
        count += 1
    
    return True

def testN4():
    n=4
    m = initMatrix(n)
    matrices = []
    getMatricesFiltered(m,n,1,1,matrices)
    matrices = [matrix.tolist() for matrix in matrices]
    filtered_eq = []
    for matrix in matrices:
        found = False
        for m in filtered_eq:
            if compareMatrices(matrix,m):
                found = True
                break

        if not found:
            filtered_eq.append(matrix)

    #matrices = [matrix for matrix in matrices if filterAbel(matrix)]
    #matrices = [fillAbel(matrix, n) for matrix in matrices]
    #matrices = [matrix for matrix in matrices if isAss(matrix, n)]
    print(len(matrices))
    print(len(filtered_eq))

def testAgain():
    m1 = [
        [0, 1, 2, 3],
        [1, 0, 3, 2],
        [2, 3, 1, 0],
        [3, 2, 0, 1]]
    m2 = [
        [0, 1, 2, 3],
        [1, 2, 3, 0],
        [2, 3, 0, 1],
        [3, 0, 1, 2]
    ]

    print(compareMatrices(m1,m2))

def why():
    m1 = [
        [0, 1, 2, 3],
        [1, 3, 0, 2],
        [2, 0, 3, 1],
        [3, 2, 1, 0]]
    m2 = [
        [0, 1, 2, 3],
        [1, 2, 3, 0],
        [2, 3, 0, 1],
        [3, 0, 1, 2]
    ]

    print(compareMatrices(m1,m2))

def ohno():
    m1 = [
        [0, 1, 2, 3],
        [1, 3, 0, 2],
        [2, 0, 3, 1],
        [3, 2, 1, 0]]
    m2 = [
        [0, 1, 2, 3],
        [1, 0, 3, 2],
        [2, 3, 1, 0],
        [3, 2, 0, 1]]

    print(compareMatrices(m1,m2))

def getMatrices(matrix, n, i, j, l):
    for val in range(0,n):
        row = 0
        while row < i and matrix[row][j] != val:
            row += 1
        
        if row == i:
            col = 0
            while col < j and matrix[i][col] != val:
                col += 1

            if col == j:
                matrix[i][j] = val
                if (i == (n-1)) and (j == (n-1)):
                    l.append(np.array(matrix))
                    continue

                if i != (n-1):
                    getMatrices(matrix, n, i+1, j, l)
                elif j != (n-1):
                    getMatrices(matrix, n, 1, j+1, l)

def getMatricesFiltered(matrix, n, i, j, l):
    for val in range(0,n):
        row = 0
        while row < i and matrix[row][j] != val:
            row += 1
        
        if row == i:
            col = 0
            while col < j and matrix[i][col] != val:
                col += 1

            if col == j:
                matrix[i][j] = val
                if (i == (n-1)) and (j == (n-1)):
                    nparr = np.array(matrix,copy=True)
                    if filterAbel(nparr):
                        if isAss(nparr):
                            l.append(nparr)
                    continue

                if i != (n-1):
                    getMatrices(matrix, n, i+1, j, l)
                elif j != (n-1):
                    getMatrices(matrix, n, 1, j+1, l)

def fillAbel(matrix, n):
    for j in range(0, n-1):
        for i in range(j+1, n):
            matrix[j][i] = matrix[i][j]
    return matrix


def initMatrix(n):
    m =[[0]*n for i in range(0,n)]
    for i in range(0,n):
        m[0][i] = i
    
    for i in range(1,n):
        m[i][0] = i

    return m

def calc(n):
    m = initMatrix(n)
    matrices = []
    getMatricesFiltered(m,n,1,1,matrices)
    matrices = [matrix.tolist() for matrix in matrices]
    filtered_eq = []
    for matrix in matrices:
        found = False
        for m in filtered_eq:
            if compareMatrices(matrix,m):
                found = True
                break

        if not found:
            filtered_eq.append(matrix)
    
    #matrices = [matrix for matrix in matrices if filterAbel(matrix)]
    #matrices = [fillAbel(matrix, n) for matrix in matrices]
    #matrices = [matrix for matrix in matrices if isAss(matrix, n)]
    print(len(filtered_eq))

def getAbelMatrices(matrix, n, i, j, l):
    for val in range(0,n):
        row = j
        while row < i and matrix[row][j] != val:
            row += 1
        
        if row == i:
            col = 0
            while col < j and matrix[i][col] != val:
                col += 1

            if col == j:
                matrix[i][j] = val
                if i == (n-1) and j == (n-1):
                    l.append(np.array(matrix))
                    continue

                if i != (n-1):
                    getAbelMatrices(matrix, n, i+1, j, l)
                elif j != (n-1):
                    getAbelMatrices(matrix, n, j+1, j+1, l)

def getMatrix(G):
    # G = (A: list of the members of the set, op: operation between the members)
    (A, op) = G
    n = len(A)
    m = initMatrix(n)
    for i in range(1,n):
        for j in range(1,n):
            m[i][j] = A.index(op(A[i],A[j]))
    
    return m

def riddle4():
    #sets
    Z2crossS3=[(0,{1:1,2:2,3:3}),(0,{1:1,2:3,3:2}),(0,{1:2,2:1,3:3}),(0,{1:3,2:2,3:1}),(0,{1:3,2:1,3:2}),(0,{1:2,2:3,3:1}),(1,{1:1,2:2,3:3}),(1,{1:1,2:3,3:2}),(1,{1:2,2:1,3:3}),(1,{1:3,2:2,3:1}),(1,{1:3,2:1,3:2}),(1,{1:2,2:3,3:1})]

    #groups
    Z12 = (list(range(0,12)), lambda x,y: (x+y)%12)
    Z3_Z4 = ([(0,0),(0,1),(0,2),(0,3),(1,0),(1,1),(1,2),(1,3),(2,0),(2,1),(2,2),(2,3)], lambda x,y: ((x[0]+y[0])%3,(x[1]+y[1])%4))
    S3 = ([{1:1,2:2,3:3},{1:1,2:3,3:2},{1:2,2:1,3:3},{1:3,2:2,3:1},{1:3,2:1,3:2},{1:2,2:3,3:1}], lambda x,y: {k:x[y[k]] for k in y})
    Z2_S3 = (Z2crossS3, lambda x,y: ((x[0]+y[0])%2,{k:x[1][y[1][k]] for k in y[1]}))
    A4 = ([{1:2,2:3,3:1,4:4},{1:3,3:2,2:1,4:4},{1:3,3:4,4:1,2:2},{1:2,2:4,4:1,3:3},{1:4,4:2,2:1,3:3},{1:4,4:3,3:1,2:2},{2:3,3:4,4:2,1:1},{2:4,4:3,3:2,1:1},{1:1,2:2,3:3,4:4},{1:2,2:1,3:4,4:3},{1:3,3:1,2:4,4:2},{1:4,4:1,2:3,3:2}], lambda x,y: {k:x[y[k]] for k in y})

    Z12Matrix = getMatrix(Z12)
    Z3_Z4Matrix = getMatrix(Z3_Z4)
    Z2_S3Matrix = getMatrix(Z2_S3)
    A4Matrix = getMatrix(A4)

    print(compareMatrices(Z12Matrix, Z3_Z4Matrix))
    print(compareMatrices(Z12Matrix, Z2_S3Matrix))
    print(compareMatrices(Z12Matrix, A4Matrix))

def riddle3():
    for n in range(4,6):
        print(f"n = {n}:")
        calc(n)

riddle3()