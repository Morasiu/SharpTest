class Test:
    def test1(self):
        print('test1')
    def test2(self, text):
        print(text)
    def test3(self, a, b):
        # Sum
        sum = a + b
        return a + b
    @staticmethod
    def test_static():
        print('static test')

class ClassTest:
    def class_test(self):
        print('class test')
    def class_test2(self):
        print('class test2')
    @staticmethod
    def class_static():
        print('static class')
