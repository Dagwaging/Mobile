#!/usr/bin/env ruby

require 'set'

require 'rubygems'
require 'faker'

$all_banner_ids = Set.new
$all_usernames = Set.new

$all_people = []
$all_faculty = []
$all_students = []

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

        if rand(5) > 1
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

        if rand(10) < 1
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
            'Moench Hall D', 'Moench Hall F', 'Crapo Hall G', 'Olin Hall O', 'HMU']
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

# Create 3 faculty
3.times do
    Faculty.new
end

# Create 30 students
30.times do
    Student.new
end

# Shuffle and write to file
$all_people.shuffle!

File.open 'small.usr', 'w' do |user_file|
    $all_people.each do |person|
        user_file.puts person.to_csv
    end
end

# Create 197 faculty
197.times do
    Faculty.new
end

# Create 2970 students
2970.times do
    Student.new
end

# Shuffle and write to second file
$all_people.shuffle!

File.open 'full.usr', 'w' do |user_file|
    $all_people.each do |person|
        user_file.puts person.to_csv
    end
end
