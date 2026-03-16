using Domain.AcademicManagement.Aggregate;
using Domain.AcademicManagement.Entity;
using Domain.CourseManagement.Aggregate;
using Domain.CourseManagement.Entity;
using Domain.IdentityManagement.Aggregate;
using Domain.IdentityManagement.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Persistence
{
    public static class Seeder
    {
        public static async Task SeedAsync(EducationPlatformDBContext context)
        {
            // ====================
            // Seed Grades
            // ====================
            var gradeIds = new Dictionary<int, Guid>();

            if (!await context.Grades.AnyAsync())
            {
                var grades = new List<Grade>();

                for (int i = 1; i <= 12; i++)
                {
                    var id = Guid.NewGuid();
                    gradeIds[i] = id;

                    grades.Add(new Grade(id, $"Grade {i}"));
                }

                context.Grades.AddRange(grades);
            }
            else
            {
                var grades = await context.Grades.ToListAsync();
                foreach (var g in grades)
                {
                    var number = int.Parse(g.Name.Split(" ")[1]);
                    gradeIds[number] = g.GradeID;
                }
            }

            // ====================
            // Seed Subjects
            // ====================
            var subjectIds = new Dictionary<string, Guid>();

            if (!await context.Subjects.AnyAsync())
            {
                var subjects = new List<Subject>();

                void AddSubject(string code, string name)
                {
                    var id = Guid.NewGuid();
                    subjectIds[code] = id;

                    subjects.Add(new Subject(id, code, name, Guid.Empty));
                }

                AddSubject("MATH", "Mathematics");
                AddSubject("VIET", "Vietnamese");
                AddSubject("ENG", "English");
                AddSubject("SCI", "Science");
                AddSubject("HIS", "History");
                AddSubject("GEO", "Geography");
                AddSubject("PHYS", "Physics");
                AddSubject("CHEM", "Chemistry");
                AddSubject("BIO", "Biology");
                AddSubject("TECH", "Technology");
                AddSubject("ART", "Art");
                AddSubject("PE", "Physical Education");
                AddSubject("CIT", "Civic Education");

                context.Subjects.AddRange(subjects);
            }
            else
            {
                var subjects = await context.Subjects.ToListAsync();
                foreach (var s in subjects)
                {
                    subjectIds[s.Code] = s.SubjectID;
                }
            }

            // ====================
            // Seed Default Lessons
            // ====================
            string[] GetTopics(string subjectCode, int grade)
            {
                return subjectCode switch
                {
                    "MATH" => grade switch
                    {
                        <= 2 => new[]
                        {
                            "Counting Numbers",
                            "Addition and Subtraction",
                            "Basic Shapes",
                            "Measuring Length",
                            "Word Problems"
                        },

                        <= 5 => new[]
                        {
                            "Multiplication and Division",
                            "Fractions",
                            "Decimals",
                            "Perimeter and Area",
                            "Math Problem Solving"
                        },

                        <= 9 => new[]
                        {
                            "Algebra Basics",
                            "Linear Equations",
                            "Geometry Fundamentals",
                            "Statistics",
                            "Graph Interpretation"
                        },

                        _ => new[]
                        {
                            "Functions",
                            "Trigonometry",
                            "Calculus Introduction",
                            "Probability",
                            "Advanced Problem Solving"
                        }
                    },

                    "VIET" => grade switch
                    {
                        <= 2 => new[]
                        {
                            "Alphabet and Pronunciation",
                            "Simple Sentences",
                            "Listening and Repeating",
                            "Reading Short Stories",
                            "Basic Writing"
                        },

                        <= 5 => new[]
                        {
                            "Reading Comprehension",
                            "Descriptive Writing",
                            "Grammar Basics",
                            "Storytelling",
                            "Vocabulary Building"
                        },

                        <= 9 => new[]
                        {
                            "Narrative Text",
                            "Poetry",
                            "Literary Analysis",
                            "Argumentative Writing",
                            "Vietnamese Grammar"
                        },

                        _ => new[]
                        {
                            "Classic Vietnamese Literature",
                            "Modern Literature",
                            "Essay Writing",
                            "Text Analysis",
                            "Critical Thinking in Literature"
                        }
                    },

                    "ENG" => new[]
                    {
                        "Vocabulary",
                        "Grammar",
                        "Reading",
                        "Listening",
                        "Writing"
                    },

                    "PHYS" => new[]
                    {
                        "Motion",
                        "Force",
                        "Energy",
                        "Electricity",
                        "Practical Applications"
                    },

                    "CHEM" => new[]
                    {
                        "Atoms and Molecules",
                        "Chemical Reactions",
                        "Periodic Table",
                        "Chemical Calculations",
                        "Chemistry in Life"
                    },

                    "BIO" => new[]
                    {
                        "Cells",
                        "Human Body",
                        "Plants",
                        "Ecosystems",
                        "Genetics Basics"
                    },

                    "HIS" => new[]
                    {
                        "Ancient Vietnam",
                        "Feudal Dynasties",
                        "Colonial Period",
                        "Modern Vietnam",
                        "World History Connections"
                    },

                    "GEO" => new[]
                    {
                        "Maps and Geography Skills",
                        "Vietnam Geography",
                        "Climate",
                        "Population",
                        "Natural Resources"
                    },

                    "SCI" => new[]
                    {
                        "Scientific Observation",
                        "Matter",
                        "Energy",
                        "Earth Science",
                        "Environment"
                    },

                    "TECH" => new[]
                    {
                        "Basic Tools",
                        "Engineering Thinking",
                        "Simple Machines",
                        "Technology in Life",
                        "Design Project"
                    },

                    "ART" => new[]
                    {
                        "Drawing Basics",
                        "Color Theory",
                        "Craft",
                        "Art Appreciation",
                        "Creative Project"
                    },

                    "PE" => new[]
                    {
                        "Warm-up Exercises",
                        "Team Sports",
                        "Fitness Training",
                        "Coordination",
                        "Health Education"
                    },

                    "CIT" => new[]
                    {
                        "Community Rules",
                        "Responsibility",
                        "Ethics",
                        "Citizenship",
                        "Life Skills"
                    },

                    _ => new[] { "Lesson 1", "Lesson 2", "Lesson 3", "Lesson 4", "Lesson 5" }
                };
            }

            if (!await context.Set<DefaultLesson>().AnyAsync())
            {
                var lessons = new List<DefaultLesson>();

                foreach (var gradePair in gradeIds)
                {
                    int gradeNumber = gradePair.Key;
                    Guid gradeId = gradePair.Value;

                    foreach (var subjectPair in subjectIds)
                    {
                        string subjectCode = subjectPair.Key;
                        Guid subjectId = subjectPair.Value;

                        var topics = GetTopics(subjectCode, gradeNumber);

                        foreach (var topic in topics)
                        {
                            lessons.Add(new DefaultLesson(
                                Guid.NewGuid(),
                                topic,
                                $"Core lesson about {topic}",
                                topic,
                                gradeId,
                                subjectId
                            ));
                        }
                    }
                }

                context.AddRange(lessons);
            }

            // ====================
            // Seed Admin User
            // ====================
            if (!await context.Users.AnyAsync(u => u.Role == Role.Admin))
            {
                var adminUser = new User(
                    userId: Guid.NewGuid(),
                    email: "longdong32120@gmail.com",
                    plainPassword: "28012005",
                    phone: "0349331141",
                    name: "Dong Xuan Bao Long",
                    bio: "Platform Administrator",
                    role: Role.Admin,
                    true
                );

                context.Users.Add(adminUser);
            }

            // ====================
            // Seed Teacher Course Review Policies (Admin Enforcement)
            // ====================
            if (!await context.Policies.AnyAsync())
            {
                var policies = new List<Policy>();
                var policyRules = new List<PolicyRule>();

                void AddPolicy(
                    string policyName,
                    (string code, string description)[] rules)
                {
                    var policyId = Guid.NewGuid();

                    policies.Add(new Policy(policyId, policyName));

                    foreach (var (code, description) in rules)
                    {
                        policyRules.Add(new PolicyRule(
                            Guid.NewGuid(),
                            code,
                            description,
                            policyId
                        ));
                    }
                }

                AddPolicy(
                    "Content Quality Policy",
                    new[]
                    {
                        ("CLEAR_DESCRIPTION", "Course description must clearly describe learning objectives."),
                        ("STRUCTURED_CONTENT", "Course content must be well-structured and organized.")
                    });

                AddPolicy(
                    "Academic Integrity Policy",
                    new[]
                    {
                        ("NO_PLAGIARISM", "Course content must not be plagiarized."),
                        ("PROPER_CITATION", "External materials must be properly cited.")
                    });

                AddPolicy(
                    "Assessment Policy",
                    new[]
                    {
                        ("ASSESSMENT_REQUIRED", "Course must include at least one assessment."),
                        ("ASSESSMENT_RELEVANT", "Assessments must align with lesson content.")
                    });

                AddPolicy(
                    "Content Safety Policy",
                    new[]
                    {
                        ("NO_OFFENSIVE_LANGUAGE", "Course must not contain offensive or abusive language."),
                        ("AGE_APPROPRIATE", "Content must be appropriate for the target age group.")
                    });

                AddPolicy(
                    "Curriculum Alignment Policy",
                    new[]
                    {
                        ("GRADE_MATCH", "Course content must match the selected grade."),
                        ("SUBJECT_MATCH", "Course content must align with the selected subject.")
                    });

                AddPolicy(
                    "Technical Quality Policy",
                    new[]
                    {
                        ("MEDIA_QUALITY", "Videos and media must meet minimum quality standards."),
                        ("NO_BROKEN_LINKS", "Course must not contain broken or missing resources.")
                    });

                AddPolicy(
                    "Pricing Policy",
                    new[]
                    {
                        ("FAIR_PRICING", "Course pricing must be reasonable."),
                        ("NO_HIDDEN_FEES", "Course must not contain hidden charges.")
                    });

                context.Policies.AddRange(policies);
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
        }
    }
}
