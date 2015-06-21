int j=2; // <TEST_EXAMPLE>
switch (level)
{
    case 1:
        {
 for (int i = 1; i <= 5; i++)
   {
          level= level+ i;
       }
return level;
          }
    case 2:
        {
          Console.WriteLine("Enter factor");
           if (factor==6)
            {
                level=level*4;
               return level;
            }
         else
         {
              level=level/2;
           return level;
          }         
        }
    case 3:
        {
       while (j > 0)
            {
                level= level-j;
                 j--;
            }
             return level*3;         
        }
    default :
        {
            if (factor==0 && k >10)
            {
                level=level*4;               
            }          
                return level/2;          
        }
}
Console.ReadLine();