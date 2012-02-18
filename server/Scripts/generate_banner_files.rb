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
    attr_accessor :major, :year, :class_year, :advisor

    def initialize
        super

        majors = ['ME', 'CS', 'SE', 'OE', 'PH', 'BE', 
            'CE', 'EE', 'CPE', 'CHE', 'CHEM', 'AB']
        if rand(6) < 1
            majors.shuffle!
            @major = "#{majors[0]}/#{majors.shuffle[1]}"
        else
            @major = majors.sample
        end

        @year = ['Y1', 'Y2', 'Y3', 'Y4', 'Y5'].sample
        @class_year = ['FR', 'SO', 'JR', 'SR', 'GR'].sample
        @advisor = $all_faculty.sample

        if chance_one_in 10
            @location = Faker::Address.street_address
        else
            halls = ['Blumberg Hall', 'Mees Hall', 'Speed Hall', 'Percopo Hall ', 
                'Apartments Hall EAST', 'Apartments Hall WEST', 'Deming Hall']
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

        @department = ['Computer Science & Software Engineering', 'Mathematics',
            'Humanities & Social Sciences', 'Physics & Optical Engineering',
            'Mechanical Engineering'].sample

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
    attr_accessor :crn, :course_number, :title, :instructor, 
        :credit_hours, :location, :final, :cap, :comments, :students

    def initialize
        @crn = rand(2000) + 2000

        while $all_crns.include? @crn
            @crn = rand(2000) + 2000
        end

        $all_crns << @crn

        @course_number = ['MA', 'PH', 'CSSE', 'BIO', 'GS', 'IA', 'RH'].sample +
            (rand(300) + 100).to_s + '-01'

        while $all_course_numbers.include? @course_number
            @course_number = ['MA', 'PH', 'CSSE', 'BIO', 'GS', 'IA', 'RH'].sample +
                (rand(300) + 100).to_s + '-01'
        end

        $all_course_numbers << @course_number

        @title = Faker::Company.catch_phrase
        @title = @title.split
        @title.each {|w| w.capitalize!}
        @title = @title.join ' '

        if chance_one_in 10
            @comments = 'Permission of instructor required'
        else
            @comments = '&nbsp'
        end

        @instructor = $all_faculty.sample
        @credit_hours = rand(5) + 1
        @location = "#{self.rand_days}/#{self.rand_hour}/#{self.rand_room}"
        @final = "#{self.rand_day}/#{rand(3) + 1}/#{self.rand_room}"

        @students = []
        @cap = rand(30)

        $all_courses << self
    end

    def enrolled
        return 20
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

    def to_csv
        return "#{@crn}|#{@course_number}|#{@title}|" +
               "#{@instructor.username}|#{@credit_hours}|#{@location}" +
               "#{@final}|#{self.enrolled}|#{@cap}|" +
               "#{@comments}|#{self.instructor_info}"
    end
end

scales = {}

if options[:sample]
    scales[:students] = 30
    scales[:courses] = 5
    scales[:faculty] = 3
else
    scales[:students] = 2000
    scales[:courses] = 500
    scales[:faculty] = 200
end

scales[:faculty].times do
    Faculty.new
end

scales[:courses].times do
    Course.new
end

scales[:students].times do
    Student.new
end

dir = DateTime.now.strftime("banner-data#{'-sample' if options[:sample]}-%Y-%m-%d-%H-%M-%S")
Dir::mkdir dir

['201110', '201120', '201130', '201210', '201220', '201230'].each do |term|
    $all_people.shuffle!
    
    File.open File.join(dir, "#{term}.usr"), 'w' do |user_file|
        $all_people.each do |person|
            user_file.puts person.to_csv
        end
    end
    
    $all_courses.shuffle!
    
    File.open File.join(dir, "#{term}.cls"), 'w' do |cls_file|
        $all_courses.each do |course|
            cls_file.puts course.to_csv
        end
    end
end

