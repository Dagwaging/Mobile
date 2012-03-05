#!/usr/bin/env ruby


require 'optparse'
require 'set'

require 'rubygems'
require 'faker'


$all_banner_ids = Set.new
$all_usernames = Set.new

$all_people = []
$all_faculty = []
$all_students = []

$all_crns = Set.new
$all_course_numbers = Set.new
$course_numbers_to_sections = {}

$all_courses = []

# Command line options
options = {}

opt_parser = OptionParser.new do |opts|
    opts.banner = 'Usage: generate_banner_data.rb [options]'

    options[:sample] = false

    opts.on('-s', '--sample', 'Only create a small number of entries') do
        options[:sample] = true
    end
end

opt_parser.parse!

def chance_one_in x
    return rand(x) < 1
end

class Person
    attr_accessor :fname, :lname, :mname, :username,
        :phone_number, :mailbox, :banner_id, :location
             
     def initialize
        @fname = Faker::Name.first_name
        @lname = Faker::Name.last_name
        @mailbox = "CM #{rand(5000)}"

        @banner_id = rand(200000000) + 800000000

        while $all_banner_ids.include? @banner_id
            @banner_id = rand(200000000) + 800000000
        end

        $all_banner_ids << @banner_id

        @phone_number = Faker::PhoneNumber.phone_number

        if chance_one_in 5
            @mname = (rand(122 - 97) + 97).chr.upcase
        else
            @mname = Faker::Name.first_name
        end

        @username = self.gen_username

        original_username = @username
        username_fixer = 0

        while $all_usernames.include? @username
            username_fixer += 1
            @username = "#{original_username}#{username_fixer}"
        end

        $all_usernames << @username
     end

     def email_address
         return "#{self.username}@rose-hulman.edu"
     end
end

class Student < Person
    attr_accessor :major, :year, :class_year, :advisor, :courses

    def initialize
        super

        majors = ['AB', 'BE', 'CE', 'CHE', 'CHEM', 'CPE',
            'CS', 'EE', 'EMGT', 'EP', 'MA', 'ME', 'OE', 'PH', 'SE']

        if rand(6) < 1
            majors.shuffle!
            @major = "#{majors[0]}/#{majors[1]}"
        else
            @major = majors.sample
        end

        @year = ['Y1', 'Y2', 'Y3', 'Y4', 'Y5'].sample
        @class_year = ['FR', 'SO', 'JR', 'SR', 'GR'].sample
        @advisor = $all_faculty.sample

        if chance_one_in 10
            @location = Faker::Address.street_address
        else
            halls = ['Blumberg Hall', 'Mees Hall', 'Scharpenburg Hall',
                'Speed Hall', 'Percopo Hall ', 'Apartments Hall EAST',
                'Apartments Hall WEST', 'Deming Hall']
            @location = "#{halls.sample} #{rand(200) + 100}"
        end

        $all_people << self
        $all_students << self
    end

    def gen_username
        return "#{@lname[0..5].gsub(/[^A-Za-z]/i, '').upcase}" +
               "#{@fname[0..0].gsub(/[^A-Za-z]/i, '').upcase}" + 
               "#{@mname[0..0].gsub(/[^A-Za-z]/i, '').upcase}"
    end

    def pick_courses
        if chance_one_in 50
            num_courses = 0
        else
            num_courses = rand(5) + 1
        end

        @courses = $all_courses.shuffle.slice 0, num_courses

        @courses.each do |course|
            course.students << self
        end
    end

    def refresh_advisor
        if not $all_faculty.include? @advisor
            @advisor = $all_faculty.sample
        end
    end

    def to_ssf_csv
        courses = @courses.map { |course| course.to_ssf_value }
        return "xxxxxx|#{@banner_id}#{'|' + courses.join('|') if not courses.empty?}"
    end

    def to_csv
        return "xxxxx|#{@banner_id}|#{@username}|" +
               "#{self.email_address}|#{@mailbox}|#{@major}|" + 
               "#{@class_year}|#{year}|#{@advisor.username}|" + 
               "#{@lname}|#{@fname}|#{@mname}|" +
               "&nbsp|#{@phone_number}|#{@location}"
    end
end

class Faculty < Person
    attr_accessor :department

    def initialize
        super

        @department = ['Applied Biology/Bio Engineering',
            'Auxiliary Enterprises', 'Chemical Engineering',
            'Chemistry & Life Sciences', 'Civil Engineering',
            'Computer Science & Software Engineering',
            'Dept of Army/Military Science',
            'Electrical and Computer Engineering',
            'Humanities & Social Sciences', 'MS-Engineering Management',
            'Mathematics', 'Mechanical Engineering',
            'Physics & Optical Engineering'].sample

        halls = ['Moench Hall A', 'Moench Hall B', 'Moench Hall C', 
            'Moench Hall D', 'Moench Hall F', 'Crapo Hall G', 'Olin Hall O', 'HMU ']
        @location = "#{halls.sample}#{rand(200) + 100}"

        $all_people << self
        $all_faculty << self
    end

    def gen_username
        return "#{@lname.gsub(/[^A-Za-z]/i, '').upcase}"
    end

    def to_csv
        return "xxxxx|#{@banner_id}|#{@username}|" +
               "#{self.email_address}|#{@mailbox}|&nbsp|" +
               "&nbsp|&nbsp|&nbsp|" +
               "#{@lname}|#{@fname}|#{@mname}|" +
               "#{@department}|#{@phone_number}|#{@location}"
    end
end

class Course
    attr_accessor :crn, :course_number, :section_number, :title, :instructor, 
        :credit_hours, :location, :final, :cap, :comments, :students

    def initialize
        @crn = rand(2000) + 2000

        while $all_crns.include? @crn
            @crn = rand(2000) + 2000
        end

        $all_crns << @crn

        @instructor = $all_faculty.sample
        if chance_one_in 10
            @location = "#{self.rand_days}/#{self.rand_hour}/#{self.rand_room}:#{self.rand_day}/#{self.rand_hour}/#{self.rand_room}"
        elsif chance_one_in 20
            @location = 'TBA/TBA/TBA'
        else
            @location = "#{self.rand_days}/#{self.rand_hour}/#{self.rand_room}"
        end

        # Sometimes just copy another course and make this another section
        if chance_one_in 5 and not $all_courses.empty?
            copied = $all_courses.sample

            $course_numbers_to_sections[copied.course_number] += 1

            @course_number = copied.course_number
            @section_number = $course_numbers_to_sections[copied.course_number]
            @title = copied.title
            @comments = copied.comments
            @credit_hours = copied.credit_hours
            @final = copied.final
            @cap = copied.cap

            @students = []

            $all_courses << self

            return
        end

        @course_number = ['AB', 'BE', 'BIO', 'CE', 'CHE',
            'CHEM', 'CSSE', 'ECE', 'EGMT', 'ES', 'GE',
            'GRAD', 'GS', 'IA', 'JP', 'MA', 'ME', 'MS', 'OE',
            'PH', 'RH', 'ROBO', 'SP', 'SV'].sample + (rand(300) + 100).to_s 

        while $all_course_numbers.include? @course_number
            @course_number = ['MA', 'PH', 'CSSE', 'BIO', 'GS', 'IA', 'RH'].sample +
                (rand(300) + 100).to_s
        end

        $all_course_numbers << @course_number

        @section_number = 1
        $course_numbers_to_sections[@course_number] = @section_number

        @title = Faker::Company.catch_phrase
        @title = @title.capitalize.gsub(/[^a-zA-Z][a-z]/i) {|s| s.upcase }

        if chance_one_in 10
            @comments = ['Permission of instructor required',
                'Junior status required', 'Second year students only'].sample
        else
            @comments = '&nbsp'
        end

        @final = "#{self.rand_day}/#{rand(3) + 1}/#{self.rand_room}"
        @credit_hours = rand(5) + 1
        @students = []
        @cap = rand(30)

        $all_courses << self
    end

    def full_course_number
        return "#{@course_number}-%02d" % @section_number
    end

    def enrolled
        return @students.count
    end

    def instructor_info
        return "#{@instructor.lname}, #{@instructor.fname} #{@instructor.mname}"
    end

    def rand_days
        if chance_one_in 5
            return 'MWT'
        elsif chance_one_in 5
            return 'W'
        elsif chance_one_in 5
            return 'TWF'
        else
            return 'MTRF'
        end
    end

    def rand_day
        return ['M', 'T', 'W', 'R', 'F'].sample
    end

    def rand_room
        return ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'O', 'M'].sample + (rand(300) + 100).to_s
    end

    def rand_hour
        if chance_one_in 5
            start = rand(9) + 1
            finish = start + rand(10 - start) + 1
            return "#{start}-#{finish}"
        else
            return rand(10) + 1
        end
    end

    def to_ssf_value
        return "#{@crn}.#{self.full_course_number}.#{@credit_hours}"
    end

    def to_csv
        return "#{@crn}|#{self.full_course_number}|#{@title}|" +
               "#{@instructor.username}|#{@credit_hours}|#{@location}|" +
               "#{@final}|#{self.enrolled}|#{@cap}|" +
               "#{@comments}|#{self.instructor_info}"
    end
end

# Set up numbers for each type of entry
if options[:sample]
    scales = {:students => 30,
              :courses => 5,
              :faculty => 3,
              :replace_students => 8,
              :replace_faculty => 2
    }
else
    scales = {:students => 2000,
              :courses => 500,
              :faculty => 200,
              :replace_students => 500,
              :replace_faculty => 20
    }
end

# Create initial set of faculty and students
scales[:faculty].times { Faculty.new }
scales[:students].times { Student.new }

# Create output directory
dir = DateTime.now.strftime("banner-data#{'-sample' if options[:sample]}-%Y-%m-%d-%H-%M-%S")
Dir::mkdir dir

['201110', '201120', '201130', '201210', '201220', '201230'].each do |term|
    # Replace faculty members
    $all_faculty.drop rand(20)
    scales[:replace_faculty].times { Faculty.new }

    # Create all new courses
    $all_courses = []
    $all_crns = []
    $all_course_numbers = []
    scales[:courses].times { Course.new }

    # Replace students
    $all_students.drop rand(500)
    scales[:replace_students].times { Student.new }

    # Check for missing advisor and register for courses
    $all_students.each do |student|
        student.refresh_advisor
        student.pick_courses
    end

    # Write .usr file
    $all_people.shuffle!
    File.open File.join(dir, "#{term}.usr"), 'w' do |user_file|
        user_file.puts term
        $all_people.each do |person|
            user_file.puts person.to_csv
        end
    end
    
    # Write .cls file
    $all_courses.shuffle!
    File.open File.join(dir, "#{term}.cls"), 'w' do |cls_file|
        cls_file.puts term
        $all_courses.each do |course|
            cls_file.puts course.to_csv
        end
    end

    # Write .ssf file
    File.open File.join(dir, "#{term}.ssf"), 'w' do |ssf_file|
        ssf_file.puts term
        $all_students.each do |student|
            ssf_file.puts student.to_ssf_csv
        end
    end
end

